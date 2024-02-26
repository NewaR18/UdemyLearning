using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using AspNetCore.Models.JSModels;
using AspNetCore.Models.ViewModel;
using AspNetCore.Utilities.Commons;
using AspNetCore.Utilities.Enumerators;
using AspNetCore.Utilities.Payments;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;

namespace AspNetCoreFromBasic.Areas.Customer.Controllers
{
	public class PaymentController : Controller
	{
		private readonly IUnitOfWork _repo;
		private readonly IConfiguration _configuration;
		private readonly EsewaPayments _esewaPayments;
		private readonly StripePayments _stripePayments;
		private readonly KhaltiPayments _khaltiPayments;
		public PaymentController(IUnitOfWork unitOfWork,
								IConfiguration configuration,
								EsewaPayments esewaPayments,
								StripePayments stripePayments,
								KhaltiPayments khaltiPayments)
		{
			_repo = unitOfWork;
			_configuration = configuration;
			_esewaPayments = esewaPayments;
			_stripePayments = stripePayments;
			_khaltiPayments = khaltiPayments;
		}
		public IActionResult Index()
		{
			return View();
		}
		public async Task<IActionResult> MakePayment()
		{
			string referer = Request.Headers["Referer"]!;
			LinkModel linkModel = GetLinkModel(referer);
			string? serializedModel = TempData["shoppingCartViewModel"] as string;
			ShoppingCartViewModel shoppingCartViewModel = JsonConvert.DeserializeObject<ShoppingCartViewModel>(serializedModel)!;
			string baseURL = _configuration["BaseURL"]!;
			if (shoppingCartViewModel.PaymentMethod == nameof(PaymentMethodEnum.Esewa))
			{
				string esewaURL = await _esewaPayments.MakePayment(shoppingCartViewModel);
				if (!string.IsNullOrEmpty(esewaURL))
				{
					Response.Headers.Add("Location", esewaURL);
					return new StatusCodeResult(303);
				}
				TempData["error"] = "Error making payment with Esewa";
			}
			else if (shoppingCartViewModel.PaymentMethod == nameof(PaymentMethodEnum.Khalti))
			{
				string khaltiURL = await _khaltiPayments.PaymentInitiate(shoppingCartViewModel);
				if (!string.IsNullOrEmpty(khaltiURL))
				{
					Response.Headers.Add("Location", khaltiURL);
					return new StatusCodeResult(303);
				}
				TempData["error"] = "Error making payment with Khalti";
			}
			else if (shoppingCartViewModel.PaymentMethod == nameof(PaymentMethodEnum.Stripe))
			{
				Session session = _stripePayments.PaymentInitiate(shoppingCartViewModel);
				if (!string.IsNullOrEmpty(session.Url))
				{
					Response.Headers.Add("Location", session.Url);
					return new StatusCodeResult(303);
				}
				TempData["error"] = "Error making payment with Scribe";
			}
			else
			{
				TempData["error"] = "Error making payment";
			}
			return RedirectToAction(linkModel.ActionName, linkModel.ControllerName, new { area = linkModel.AreaName, id = linkModel.QueryId});
		}
        public IActionResult StartRefund()
        {
            string referer = Request.Headers["Referer"]!;
            LinkModel linkModel = GetLinkModel(referer);
            string? serializedModel = TempData["orderHeaderPayViewModel"] as string;
            OrderHeaderPayViewModel orderHeaderPayViewModel = JsonConvert.DeserializeObject<OrderHeaderPayViewModel>(serializedModel)!;
            string baseURL = _configuration["BaseURL"]!;
            if (orderHeaderPayViewModel.PaymentMethod == nameof(PaymentMethodEnum.Esewa))
            {
                _esewaPayments.StartRefundProcess(orderHeaderPayViewModel);
                TempData["success"] = $"Refund Process Initiated with {nameof(PaymentMethodEnum.Esewa)}";
            }
            else if (orderHeaderPayViewModel.PaymentMethod == nameof(PaymentMethodEnum.Khalti))
            {
                _khaltiPayments.StartRefundProcess(orderHeaderPayViewModel);
                TempData["success"] = $"Refund Process Initiated with {nameof(PaymentMethodEnum.Khalti)}";
            }
            else if (orderHeaderPayViewModel.PaymentMethod == nameof(PaymentMethodEnum.Stripe))
            {
                _stripePayments.StartRefundProcess(orderHeaderPayViewModel);
                TempData["success"] = $"Refund Process Initiated with {nameof(PaymentMethodEnum.Stripe)}";
            }
            else
            {
                TempData["error"] = "Error making payment";
            }
            return RedirectToAction(linkModel.ActionName, linkModel.ControllerName, new { area = linkModel.AreaName, id = linkModel.QueryId });
        }

        #region APIS
        public LinkModel GetLinkModel(string referer)
		{
			var refererUri = new Uri(referer);
			var queryString = QueryHelpers.ParseQuery(refererUri.Query);
			var pathSegments = refererUri.AbsolutePath.Trim('/').Split('/');

			LinkModel linkModel = new LinkModel()
			{
				ControllerName = pathSegments.Length > 1 ? pathSegments[1] : null,
				ActionName = pathSegments.Length > 2 ? pathSegments[2] : null,
				AreaName = pathSegments.Length > 0 ? pathSegments[0] : null,
				QueryId = queryString["id"]
			};
			return linkModel;	
		}
		#endregion

	}
}
