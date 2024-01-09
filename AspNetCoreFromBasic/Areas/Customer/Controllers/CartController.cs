using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using AspNetCore.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AspNetCoreFromBasic.Areas.Customer.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _repo;
        public ShoppingCartViewModel ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _repo = unitOfWork;
        }
        public IActionResult Index()
        {
            var userClaimsIdentity = (ClaimsIdentity)User.Identity!;
            var userIdClaim = userClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                string userId = userIdClaim.Value;
                ShoppingCartVM = new ShoppingCartViewModel()
                {
                    ListCart = _repo.ShoppingCartRepo.GetAll(x => x.ApplicationUserId.Equals(userId), IncludeProperties: "Product")
                };
                return View(ShoppingCartVM);
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
