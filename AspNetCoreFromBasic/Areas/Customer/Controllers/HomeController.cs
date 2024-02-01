using AspNetCore.DataAccess.Migrations;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;

namespace AspNetCoreFromBasic.Areas.Customer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _repo;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public IActionResult Index()
        {
            var controllerTypes = Assembly.GetExecutingAssembly()
                                           .GetTypes();
                                          // .Where(type => typeof(Controller).IsAssignableFrom(type) && type.Name.EndsWith("Controller"));
            IEnumerable<Product> entities = _repo.ProductRepo.GetAll(IncludeProperties: "Category,CoverType");
            return View(entities);
        }
        public IActionResult Details(int productId)
        {
            if(productId == 0)
            {
                TempData["error"] = "Problem occurred with Accessing Details Page";
                return RedirectToAction(nameof(Index));
            }
            ShoppingCart cartObj = new()
            {
                ProductId = productId,
                Product = _repo.ProductRepo.GetFirstOrDefault(x => x.Id == productId, IncludeProperties: "Category,CoverType"),
                Count = 1
            };
            return View(cartObj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                var userClaimsIdentity = (ClaimsIdentity)User.Identity!;
                var userClaims = userClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaims == null)
                {
                    TempData["error"] = "Error finding User";
                    return RedirectToAction(nameof(Index));
                }
                shoppingCart.ApplicationUserId = userClaims.Value;
                ShoppingCart existingShoppingCart = _repo.ShoppingCartRepo.GetFirstOrDefault(x => x.ApplicationUserId.Equals(userClaims.Value) && x.ProductId.Equals(shoppingCart.ProductId));
                if (existingShoppingCart != null)
                {
                    _repo.ShoppingCartRepo.IncrementCount(existingShoppingCart, shoppingCart.Count);
                }
                else
                {
                    _repo.ShoppingCartRepo.Add(shoppingCart);
                }
                _repo.Save();
                TempData["success"] = "Product Added To Cart Successfully";
                return RedirectToAction(nameof(Details), new { productId = shoppingCart.ProductId });
            }
            shoppingCart.Product = _repo.ProductRepo.GetFirstOrDefault(x => x.Id == shoppingCart.ProductId, IncludeProperties: "Category,CoverType");
            return View(shoppingCart);
        }
        public IActionResult BuyNow(int id, int count) 
        {
            ShoppingCart cartObj = new()
            {
                Product = _repo.ProductRepo.GetFirstOrDefault(x => x.Id == id, IncludeProperties: "Category,CoverType"),
                Count = count
            };
            return View(cartObj);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}