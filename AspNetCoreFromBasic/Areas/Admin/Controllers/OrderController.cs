using AspNetCore.DataAccess.Repository;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using AspNetCore.Models.JSModels;
using AspNetCore.Models.ViewModel;
using AspNetCore.Utilities.EmailConfigurations;
using AspNetCore.Utilities.Enumerators;
using AspNetCore.Utilities.ManageBackgroundJobs;
using AspNetCoreFromBasic.Areas.Customer.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Stripe;
using System.Security.Claims;

namespace AspNetCoreFromBasic.Areas.Admin.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _repo;
        private readonly IEmailSender _emailSender;
        private readonly ManageHangfireJobs _hangFireJob;
        public OrderController(IUnitOfWork repo, IEmailSender emailSender,ManageHangfireJobs hangFireJob)
        {
            _repo = repo;
            _emailSender = emailSender;
            _hangFireJob = hangFireJob;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int Id) 
        {
            OrderHeader orderHeader = _repo.OrderHeaderRepo.GetFirstOrDefault(s => s.Id == Id);
			OrderHeaderPayViewModel orderHeaderPayViewModel = new OrderHeaderPayViewModel()
            {
                OrderHeader = orderHeader,
                PaymentMethod = "Esewa"
            };
            if(orderHeader == null)
            {
                return View(nameof(Index));
            }
            return View(orderHeaderPayViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetail(OrderHeaderPayViewModel orderHeaderPayViewModel)
        {
            OrderHeader orderHeader = _repo.OrderHeaderRepo.GetFirstOrDefault(u => u.Id.Equals(orderHeaderPayViewModel.OrderHeader.Id));

            orderHeader.PhoneNumber = orderHeaderPayViewModel.OrderHeader.PhoneNumber;
            orderHeader.StreetAddress = orderHeaderPayViewModel.OrderHeader.StreetAddress;
            orderHeader.City = orderHeaderPayViewModel.OrderHeader.City;
            orderHeader.State = orderHeaderPayViewModel.OrderHeader.State;
            orderHeader.PostalCode = orderHeaderPayViewModel.OrderHeader.PostalCode;
            orderHeader.Name = orderHeaderPayViewModel.OrderHeader.Name;
            if(!string.IsNullOrEmpty(orderHeaderPayViewModel.OrderHeader.Carrier))
            {
                orderHeader.Carrier = orderHeaderPayViewModel.OrderHeader.Carrier;
            }
			if (!string.IsNullOrEmpty(orderHeaderPayViewModel.OrderHeader.TrackingNumber))
			{
				orderHeader.TrackingNumber = orderHeaderPayViewModel.OrderHeader.TrackingNumber;
			}
			_repo.Save();
            TempData["success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details),"Order", new {Id=orderHeader.Id});
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartProcessing(OrderHeaderPayViewModel orderHeaderPayViewModel)
		{
            _repo.OrderHeaderRepo.UpdateStatus(orderHeaderPayViewModel.OrderHeader.Id, nameof(OrderEnum.Processing));
			_repo.Save();
			TempData["success"] = "Order Details Updated Successfully";
			return RedirectToAction(nameof(Details), "Order", new { Id = orderHeaderPayViewModel.OrderHeader.Id });
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ShipOrder(OrderHeaderPayViewModel orderHeaderPayViewModel)
		{
			OrderHeader orderHeader = _repo.OrderHeaderRepo.GetFirstOrDefault(u => u.Id.Equals(orderHeaderPayViewModel.OrderHeader.Id));
			orderHeader.TrackingNumber = orderHeaderPayViewModel.OrderHeader.TrackingNumber;
			orderHeader.Carrier = orderHeaderPayViewModel.OrderHeader.Carrier;
			orderHeader.OrderStatus = nameof(OrderEnum.Shipped);
			orderHeader.ShippingDate = DateTime.Now;
            string subject = "Order Shipping Started";
            string emailBody = $"<p>Dear {orderHeaderPayViewModel.OrderHeader.Name},</p>\r\n<br/>\r\n<p>The Order Shipping has been started. Your Carrier is {orderHeader.Carrier}. Please use following Tracking Number to Track Your Order.</p>\r\n<br/>\r\n<p>{orderHeader.TrackingNumber}</p>\r\n<br/>\r\n<p>Best Regards</p>\r\n<p>Sudip Shrestha</p>";
            _hangFireJob.AddEmailJobToQueue(orderHeader.ApplicationUser.Email, subject, emailBody);
            //_emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "Order Shipping Started", emailBody);
            _repo.Save();
			TempData["success"] = "Order Details Updated Successfully";
			return RedirectToAction(nameof(Details), "Order", new { Id = orderHeaderPayViewModel.OrderHeader.Id });
		}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder(OrderHeaderPayViewModel orderHeaderPayViewModel)
        {
            OrderHeader orderHeader = _repo.OrderHeaderRepo.GetFirstOrDefault(u => u.Id.Equals(orderHeaderPayViewModel.OrderHeader.Id));
            if(orderHeader.PaymentStatus == nameof(PaymentEnum.Approved))
            {
                if (string.IsNullOrEmpty(orderHeader.SessionId))
                {
                    TempData["success"] = "Payment was not found. Please contact support to proceed";
                    return RedirectToAction(nameof(Details), "Order", new { Id = orderHeaderPayViewModel.OrderHeader.Id });
                }
                if (orderHeader.SessionId == nameof(PaymentMethodEnum.Esewa))
                {
                    orderHeaderPayViewModel.PaymentMethod = nameof(PaymentMethodEnum.Esewa);
                    string serializedModel = JsonConvert.SerializeObject(orderHeaderPayViewModel);
                    TempData["orderHeaderPayViewModel"] = serializedModel;
                    return RedirectToAction("StartRefund", "Payment", new { area = "Customer" });
                }
                else if(orderHeader.SessionId == nameof(PaymentMethodEnum.Khalti))
                {
                    orderHeaderPayViewModel.PaymentMethod = nameof(PaymentMethodEnum.Khalti);
                    string serializedModel = JsonConvert.SerializeObject(orderHeaderPayViewModel);
                    TempData["orderHeaderPayViewModel"] = serializedModel;
                    return RedirectToAction("StartRefund", "Payment", new { area = "Customer" });
                }
                else
                {
                    orderHeaderPayViewModel.PaymentMethod = nameof(PaymentMethodEnum.Stripe);
                    string serializedModel = JsonConvert.SerializeObject(orderHeaderPayViewModel);
                    TempData["orderHeaderPayViewModel"] = serializedModel;
                    return RedirectToAction("StartRefund", "Payment", new { area = "Customer" });
                }
            }
            else
            {
                _repo.OrderHeaderRepo.UpdateStatus(orderHeader.Id,nameof(OrderEnum.Cancelled),nameof(PaymentEnum.Cancelled));
            }
            _repo.Save();
            TempData["success"] = "Order Details Updated Successfully";
            return RedirectToAction(nameof(Details), "Order", new { Id = orderHeaderPayViewModel.OrderHeader.Id });
        }
        public IActionResult PayNow(OrderHeaderPayViewModel orderHeaderPayViewModel)
		{
            try
            {
                OrderHeader orderHeader = _repo.OrderHeaderRepo.GetFirstOrDefault(x => x.Id.Equals(orderHeaderPayViewModel.OrderHeader.Id));
                orderHeaderPayViewModel.OrderHeader.OrderTotal = orderHeader.OrderTotal;
				orderHeaderPayViewModel.OrderHeader.ApplicationUserId = orderHeader.ApplicationUserId;
				IEnumerable<OrderDetails> orderDetails=_repo.OrderDetailsRepo.GetAll(x=>x.OrderId == orderHeaderPayViewModel.OrderHeader.Id,IncludeProperties: "Product");
                IEnumerable<ShoppingCart> listCarts = new List<ShoppingCart>();
                listCarts = orderDetails.Select(
                    u => new ShoppingCart
                    {
                        ProductId = u.ProductId,
                        Count = u.Count,
                        Price = u.Price,
                        Product = u.Product
                    }
                );
                ShoppingCartViewModel shoppingCartViewModel = new ShoppingCartViewModel()
                {
                    OrderHeader = orderHeaderPayViewModel.OrderHeader,
                    ListCart = listCarts,
					PaymentMethod = orderHeaderPayViewModel.PaymentMethod
                };
				string serializedModel = JsonConvert.SerializeObject(shoppingCartViewModel);
				TempData["shoppingCartViewModel"] = serializedModel;
			}
            catch (Exception ex)
            {
                throw ex;
            }
			return RedirectToAction("MakePayment", "Payment", new { area = "Customer"});
		}
		//In Default, It has local pagination. And For filter and pagination, it does not hit backend.
		//But Using processing: true,serverSide: true   -- It hits backend on every pagination and filter.
		#region API
		[HttpPost]
        public JsonResult GetAll([FromBody] DataTableAjaxModel model)
        {
            var TotalRecord = 0;
            if (User.IsInRole("Admin") || User.IsInRole("Super Admin"))
            {
                TotalRecord = _repo.OrderHeaderRepo.GetCount();
            }
            else
            {
                var userClaimsIdentity = (ClaimsIdentity)User.Identity!;
                var userIdClaim = userClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                TotalRecord = _repo.OrderHeaderRepo.GetCountOfUser(userIdClaim.Value);
            }

            PaginatedOrderHeader entities = _repo.OrderHeaderRepo.GetPaginatedRows(model,User);
            var responseData = new
            {
                draw = model.Draw,
                recordsTotal = TotalRecord, 
                recordsFiltered = entities.TotalCount, 
                data = entities.Data
            };
            return Json(responseData);
        }
        #endregion
    }
}
