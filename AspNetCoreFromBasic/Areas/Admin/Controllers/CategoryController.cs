using AspNetCore.DataAccess.Repository;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreFromBasic.Areas.Admin.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _repo;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _repo = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> entities = _repo.CategoryRepo.GetAll();
            return View(entities);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category entity)
        {

            if (ModelState.IsValid)
            {
                _repo.CategoryRepo.Add(entity);
                _repo.Save();
                TempData["success"] = "Item Created Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Item could not be created !! Validation error";
            return View();
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            Category entity = _repo.CategoryRepo.GetFirstOrDefault(x => x.Id == id);
            return View(entity);
        }
        public IActionResult Edit(int id)
        {
            Category entity = _repo.CategoryRepo.GetFirstOrDefault(x => x.Id == id);
            return View(entity);
        }
        [HttpPost]
        public IActionResult Edit(Category entity)
        {
            _repo.CategoryRepo.Update(entity);
            _repo.Save();
            TempData["success"] = "Item Updated Successfully";
            return RedirectToAction("Index");
        }
        #region API
        public IActionResult Delete(int id)
        {
            Product product = _repo.ProductRepo.GetFirstOrDefault(x => x.CategoryId == id);
            if (product == null)
            {
                if (id == 0)
                {
                    return Json(new { success = false, message = "Error" });

                }
                else
                {
                    Category entity = _repo.CategoryRepo.GetFirstOrDefault(x => x.Id == id);
                    _repo.CategoryRepo.Remove(entity);
                    _repo.Save();
                    return Json(new { success = true, message = "Item Deleted Successfully" });
                }
            }
            else
            {
                return Json(new { success = false, message = "Following Covertype is associated with Product" });
            }
        }
        #endregion
    }
}
