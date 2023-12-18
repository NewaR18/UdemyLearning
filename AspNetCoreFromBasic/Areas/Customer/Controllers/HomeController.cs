using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            IEnumerable<Product> entities = _repo.ProductRepo.GetAll(IncludeProperties: "Category,CoverType");
            return View(entities);
        }
        public IActionResult Details(int? id)
        {
            ShoppingCart cartObj = new()
            {
                Product = _repo.ProductRepo.GetFirstOrDefault(x => x.Id == id, IncludeProperties: "Category,CoverType"),
                Count = 1
            };
            return View(cartObj);
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