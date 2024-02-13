using AspNetCore.DataAccess.Migrations;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using AspNetCore.Models.ViewModel;
using AspNetCore.Utilities.ApiGateway;
using AspNetCore.Utilities.Commons;
using AspNetCore.Utilities.Enumerators;
using AspNetCore.Utilities.Payments;
using AspNetCore.Utilities.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Claims;

namespace AspNetCoreFromBasic.Areas.Customer.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _repo;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IConfiguration _configuration;
		private readonly EsewaPayments _esewaPayments;
		private readonly StripePayments _stripePayments;
		private readonly KhaltiPayments _khaltiPayments;
		private readonly SMSSending _smsSending;
		public CartController(IUnitOfWork unitOfWork, 
								UserManager<ApplicationUser> userManager,
								IConfiguration configuration, 
								EsewaPayments esewaPayments, 
								StripePayments stripePayments,
								KhaltiPayments khaltiPayments,
								SMSSending smsSending)
        {
            _repo = unitOfWork;
			_userManager = userManager;
			_configuration = configuration;
			_esewaPayments = esewaPayments;
			_stripePayments = stripePayments;
			_khaltiPayments = khaltiPayments;
			_smsSending = smsSending;
        }
        public IActionResult Index()
        {
			var userClaimsIdentity = (ClaimsIdentity)User.Identity!;
            var userIdClaim = userClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                string userId = userIdClaim.Value;
				ShoppingCartViewModel ShoppingCartVM = new ShoppingCartViewModel()
                {
                    ListCart = _repo.ShoppingCartRepo.GetAll(x => x.ApplicationUserId.Equals(userId) && x.Count>0, IncludeProperties: "Product"),
					OrderHeader = new()
                };
				foreach(var item in ShoppingCartVM.ListCart)
				{
					item.Price = GetPriceBasedOnCount(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
					ShoppingCartVM.OrderHeader.OrderTotal += item.Count*item.Price;
                }
                return View(ShoppingCartVM);
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
		public async Task<IActionResult> Summary()
		{
			var userClaimsIdentity = (ClaimsIdentity)User.Identity!;
			var userIdClaim = userClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			if (userIdClaim != null)
			{
				string userId = userIdClaim.Value;
				var User = await _userManager.FindByIdAsync(userId);
				ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel()
				{
					ListCart = _repo.ShoppingCartRepo.GetAll(x => x.ApplicationUserId.Equals(userId) && x.Count > 0, IncludeProperties: "Product"),
					OrderHeader = new OrderHeader()
					{
						ApplicationUser = User,
						ApplicationUserId = userId,
						Name = User.Name,
						PhoneNumber = User.PhoneNumber,
						StreetAddress = User.Address
					}
				};
				foreach (var item in shoppingCartViewModel.ListCart)
				{
					item.Price = GetPriceBasedOnCount(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
					shoppingCartViewModel.OrderHeader.OrderTotal += item.Count * item.Price;
				}
				return View(shoppingCartViewModel);
			}
			return RedirectToAction(nameof(Index));
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Summary")]
		public async Task<IActionResult> SummaryPost(ShoppingCartViewModel shoppingCartViewModel)
		{
			if(!ModelState.IsValid)
			{
				return RedirectToAction("Summary");
			}
			var userClaimsIdentity = (ClaimsIdentity)User.Identity!;
			var userIdClaim = userClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			if (userIdClaim != null)
			{
				try
				{
					_repo.BeginTransaction();
					string userId = userIdClaim.Value;
					var User = await _userManager.FindByIdAsync(userId);
					shoppingCartViewModel.ListCart = _repo.ShoppingCartRepo
													.GetAll(x => x.ApplicationUserId.Equals(userId) && x.Count > 0,
													IncludeProperties: "Product");
					foreach (var item in shoppingCartViewModel.ListCart)
					{
						item.Price = GetPriceBasedOnCount(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
						shoppingCartViewModel.OrderHeader.OrderTotal += item.Count * item.Price;
					}
					shoppingCartViewModel.OrderHeader.ApplicationUserId = userId;
					shoppingCartViewModel.OrderHeader.PaymentStatus = nameof(PaymentEnum.Pending);
					shoppingCartViewModel.OrderHeader.OrderStatus = nameof(OrderEnum.Pending);
					shoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;
					_repo.OrderHeaderRepo.Add(shoppingCartViewModel.OrderHeader);
					_repo.Save();
					foreach (var item in shoppingCartViewModel.ListCart)
					{
						OrderDetails orderDetails = new OrderDetails()
						{
							OrderId = shoppingCartViewModel.OrderHeader.Id,
							ProductId = item.ProductId,
							Price = item.Price,
							Count = item.Count
						};
						_repo.OrderDetailsRepo.Add(orderDetails);
					}
					_repo.Save();
					_repo.CommitTransaction();
				}
				catch (Exception)
				{
					_repo.RollbackTransaction();
					throw;
				}
				finally
				{
					_repo.DisposeTransaction();
				}
			}
			return await MakePayment(shoppingCartViewModel);
		}
		public async Task<IActionResult> MakePayment(ShoppingCartViewModel shoppingCartViewModel)
		{
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
			else if (shoppingCartViewModel.PaymentMethod == nameof(PaymentMethodEnum.Scribe))
			{
				Session session = _stripePayments.PaymentInitiate(shoppingCartViewModel);
				if (!string.IsNullOrEmpty(session.Url))
				{
					Response.Headers.Add("Location", session.Url);
					return new StatusCodeResult(303);
				}
				TempData["error"] = "Error making payment with Scribe";
			}
			return RedirectToAction(nameof(Summary));
		}
		public IActionResult StripeSuccess(int Id)
		{
			_stripePayments.OnSuccess(Id);
			ClearShoppingCart();
			return View("OrderConfirmation", Id);
		}
		public IActionResult StripeFailure()
		{
			TempData["error"] = "Payment Failed from Stripe";
			return RedirectToAction(nameof(Summary));
		}
		public IActionResult EsewaSuccess(string data)
		{
			int OrderId=_esewaPayments.OnSuccess(data);
			ClearShoppingCart();
			return View("OrderConfirmation",OrderId);
		}
		public IActionResult EsewaFailure(string data)
		{
			TempData["error"] = "Payment Failed from Esewa";
			return RedirectToAction(nameof(Summary));
		}
		public IActionResult KhaltiSuccess(PaymentKhaltiResponse khaltiPaymentResponse)
		{
			if (string.IsNullOrEmpty(khaltiPaymentResponse.txnId))
			{
				TempData["error"] = "Payment Failed from Khalti";
				return RedirectToAction(nameof(Summary));
			}
			int OrderId = _khaltiPayments.OnSuccess(khaltiPaymentResponse);
			ClearShoppingCart();
			return View("OrderConfirmation", OrderId);
		}
		public IActionResult KhaltiFailure(PaymentKhaltiResponse khaltiPaymentResponse)
		{
			TempData["error"] = "Payment Failed from Khalti";
			return RedirectToAction(nameof(Summary));
		}
		public void ClearShoppingCart()
		{
			var userClaimsIdentity = (ClaimsIdentity)User.Identity!;
			var userIdClaim = userClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			string userId = userIdClaim.Value;
			IEnumerable<ShoppingCart> shoppingCartList = _repo.ShoppingCartRepo.
																GetAll(x => x.ApplicationUserId.Equals(userId));
			_repo.ShoppingCartRepo.RemoveRange(shoppingCartList.ToList());
			_repo.Save();
		}

		#region Cart Apis
		[HttpPost]
		public JsonResult ChangeCount(int productId, int count)
		{
			var userClaimsIdentity = (ClaimsIdentity)User.Identity!;
			var userIdClaim = userClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			if(productId == 0 ||  count == 0)
			{
				return Json(new { success = false, message = "Error occurred" });
			}
            if (userIdClaim != null)
            {
                string userId = userIdClaim.Value;
                var cartData = _repo.ShoppingCartRepo.GetFirstOrDefault(x => x.ApplicationUserId.Equals(userId) && x.ProductId.Equals(productId));
                cartData.Count = count;
                _repo.Save();
				return Json(new { success = true, message = "Cart Quantity Updated Successfully" });
			}
            return Json(new { success = false, message = "Error occurred!! Please login again to continue" });
		}

		public JsonResult Delete(int productId)
		{
			var userClaimsIdentity = (ClaimsIdentity)User.Identity!;
			var userIdClaim = userClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			if (userIdClaim != null)
			{
				string userId = userIdClaim.Value;
				var cartData = _repo.ShoppingCartRepo.GetFirstOrDefault(x => x.ApplicationUserId.Equals(userId) && x.ProductId.Equals(productId));
				if (cartData == null)
				{
					return Json(new { success = false, message = "Could not remove the product from Cart" });
				}
				_repo.ShoppingCartRepo.Remove(cartData);
				_repo.Save();
				return Json(new { success = true, message = "Product Removed From Cart" });
			}
			return Json(new { success = true, message = "Error occurred!! Please login again to continue" });
		}
		public decimal GetPriceBasedOnCount(int count, decimal price, decimal price50, decimal price100)
		{
			if(count < 50) 
			{
				return price;
			}
			if(count < 100)
			{
				return price50;
			}
			return price100;
		}
		
		#endregion
	}
}
