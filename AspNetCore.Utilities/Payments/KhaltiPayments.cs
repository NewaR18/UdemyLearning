using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using AspNetCore.Models.ViewModel;
using AspNetCore.Utilities.ApiGateway;
using AspNetCore.Utilities.Enumerators;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Utilities.Payments
{
	public class KhaltiPayments
	{
		private readonly IUnitOfWork _repo;
		private readonly IConfiguration _configuration;
		private readonly UserManager<ApplicationUser> _userManager;
		public KhaltiPayments(IUnitOfWork repo, IConfiguration configuration, UserManager<ApplicationUser> userManager)
		{
			_repo = repo;
			_userManager = userManager;
			_configuration = configuration;
		}
		public async Task<string> PaymentInitiate(ShoppingCartViewModel shoppingCartViewModel)
		{
			var user = await _userManager.FindByIdAsync(shoppingCartViewModel.OrderHeader.ApplicationUserId);
			var url = _configuration["Khalti:Url"]!;
			var UniqueTransactionId = Guid.NewGuid().ToString() + "_" + shoppingCartViewModel.OrderHeader.Id.ToString();
			string TotalAmount = Convert.ToString(shoppingCartViewModel.OrderHeader.OrderTotal * 100);
			var successURL = APIGateway.KhaltiSuccess;
			var failureURL = APIGateway.KhaltiFailure;
			var productDetails = shoppingCartViewModel.ListCart.Select(cartItem => new
			{
				identity = cartItem.ProductId,
				name = cartItem.Product.Title,
				total_price = cartItem.Price * cartItem.Count * 100,
				quantity = cartItem.Count,
				unit_price = cartItem.Price * 100
			}).ToArray();
			var payload = new
			{
				return_url = successURL,
				website_url = failureURL,
				amount = TotalAmount,
				purchase_order_id = UniqueTransactionId,
				purchase_order_name = "Cart Total",
				customer_info = new
				{
					name = user.Name,
					email = user.Email,
					phone = user.PhoneNumber
				},
				amount_breakdown = new[]
				{
					new { label = "Total Price", amount = TotalAmount }
				},
				product_details = productDetails
			};

			var jsonPayload = JsonConvert.SerializeObject(payload);
			var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
			var client = new HttpClient();
			var liveSecretKey = _configuration["Khalti:SecretKey"]!;
			client.DefaultRequestHeaders.Add("Authorization", $"Key {liveSecretKey}");
			var response = await client.PostAsync(url, content);
			if (response.StatusCode == HttpStatusCode.OK)
			{
				var responseContent = await response.Content.ReadAsStringAsync();
				SuccessResponseKhalti successResponseKhalti = JsonConvert.DeserializeObject<SuccessResponseKhalti>(responseContent)!;
				if (successResponseKhalti != null)
				{
					PaymentKhalti khaltiPayment = new PaymentKhalti()
					{
						Pidx = successResponseKhalti.pidx,
						Amount = Convert.ToDecimal(TotalAmount),
						Status = "INITIATED"
					};
					_repo.PaymentKhaltiRepo.Add(khaltiPayment);
					_repo.Save();
					return successResponseKhalti.payment_url;
				}
			}
			return "";
		}
		public int OnSuccess(PaymentKhaltiResponse khaltiPaymentResponse)
		{
			PaymentKhalti paymentKhalti = _repo.PaymentKhaltiRepo.GetFirstOrDefault(x => x.Pidx == khaltiPaymentResponse.pidx);
			paymentKhalti.Amount = khaltiPaymentResponse.amount / 100;
			paymentKhalti.Status = khaltiPaymentResponse.status;
			paymentKhalti.MobileNo = khaltiPaymentResponse.mobile;
			paymentKhalti.TxnId = khaltiPaymentResponse.tidx;
			paymentKhalti.PurchaseOrderId = khaltiPaymentResponse.purchase_order_id;
			paymentKhalti.PurchaseOrderName = khaltiPaymentResponse.purchase_order_name;
			_repo.Save();
			int OrderId = Convert.ToInt32(khaltiPaymentResponse.purchase_order_id.Split('_').Last());
			var order = _repo.OrderHeaderRepo.GetFirstOrDefault(x => x.Id == OrderId);
			_repo.OrderHeaderRepo.UpdateStatus(OrderId, nameof(OrderEnum.Approved), nameof(PaymentEnum.Approved));
			_repo.OrderHeaderRepo.UpdateStripeData(OrderId, "Khalti", khaltiPaymentResponse.tidx);
			_repo.Save();
			return OrderId;
		}
        public void StartRefundProcess(OrderHeaderPayViewModel orderHeaderPayViewModel)
        {
            //Refund Logic not available for Esewa and Khalti, Gotta provide refund manually
            PaymentRefund paymentRefund = new PaymentRefund()
            {
                Id = 0,
                PaymentOptions = nameof(PaymentMethodEnum.Khalti),
                Currency = "npr",
                CreatedDate = DateTime.Now,
                Reason = "requested_by_customer",
                Status = "notRefunded",
                PaymentIntentId = orderHeaderPayViewModel.OrderHeader.PaymentIntentId,
                StripeId = "",
                BalanceTransactionId = "",
                ChargeId = "",
                RequestId = ""
            };
            _repo.PaymentRefundRepo.Add(paymentRefund);
            _repo.Save();
            _repo.OrderHeaderRepo.UpdateStatus(orderHeaderPayViewModel.OrderHeader.Id, nameof(OrderEnum.Cancelled), nameof(PaymentEnum.Refunded));
            _repo.Save();
        }
    }
}
