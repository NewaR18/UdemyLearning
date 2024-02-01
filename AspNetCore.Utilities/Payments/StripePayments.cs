using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using AspNetCore.Models.ViewModel;
using AspNetCore.Utilities.ApiGateway;
using AspNetCore.Utilities.Enumerators;
using AspNetCore.Utilities.Security;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Utilities.Payments
{
	public class StripePayments
	{
		private readonly IUnitOfWork _repo;
		public StripePayments(IUnitOfWork repo) {
			_repo = repo;
		}

		public Session PaymentInitiate(ShoppingCartViewModel shoppingCartViewModel)
		{
			var options = new SessionCreateOptions
			{
				LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
				SuccessUrl = APIGateway.StripeSuccess+$"?Id={shoppingCartViewModel.OrderHeader.Id}",
				CancelUrl = APIGateway.StripeFailure,
				Currency = "NPR",
			};
			foreach (var item in shoppingCartViewModel.ListCart) 
			{
				var sessionLineItemOptions = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Price*100),
						Currency = "NPR",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Title,
						},
					},
					Quantity = item.Count
				};
				options.LineItems.Add(sessionLineItemOptions);
			}
			var service = new SessionService();
			Session session = service.Create(options);
			_repo.OrderHeaderRepo.UpdateStripeData(shoppingCartViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
			_repo.Save();
			return session;
		}
		public void OnSuccess(int Id)
		{
			var order = _repo.OrderHeaderRepo.GetFirstOrDefault(x => x.Id == Id);
			var service = new SessionService();
			Session session = service.Get(order.SessionId);
			if (string.Equals(session.PaymentStatus, "paid", StringComparison.OrdinalIgnoreCase))
			{
				_repo.OrderHeaderRepo.UpdateStatus(Id, nameof(OrderEnum.Approved), nameof(PaymentEnum.Approved));
				_repo.OrderHeaderRepo.UpdateStripeData(Id, session.Id, session.PaymentIntentId);
				_repo.Save();
			}
		}
	}
}
