using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreFromBasic.Areas.Admin.Controllers
{
    public class MenuController : Controller
    {
        private readonly IUnitOfWork _repo;
        public MenuController(IUnitOfWork unitOfWork)
        {
            _repo = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<Menu> entities = _repo.MenuRepo.GetAll();
            return View(entities);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Menu entity)
        {

            if (ModelState.IsValid)
            {
                _repo.MenuRepo.Add(entity);
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
            Menu entity = _repo.MenuRepo.GetFirstOrDefault(x => x.MenuId == id);
            return View(entity);
        }
        public IActionResult Edit(int id)
        {
            Menu entity = _repo.MenuRepo.GetFirstOrDefault(x => x.MenuId == id);
            return View(entity);
        }
        [HttpPost]
        public IActionResult Edit(Menu entity)
        {
            _repo.MenuRepo.Update(entity);
            _repo.Save();
            TempData["success"] = "Item Updated Successfully";
            return RedirectToAction("Index");
        }
        //#region API
        //public IActionResult Delete(int id)
        //{
        //    Product product = _repo.ProductRepo.GetFirstOrDefault(x => x.MenuId == id);
        //    if (product == null)
        //    {
        //        if (id == 0)
        //        {
        //            return Json(new { success = false, message = "Error" });

        //        }
        //        else
        //        {
        //            Category entity = _repo.CategoryRepo.GetFirstOrDefault(x => x.Id == id);
        //            _repo.CategoryRepo.Remove(entity);
        //            _repo.Save();
        //            return Json(new { success = true, message = "Item Deleted Successfully" });
        //        }
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = "Following Covertype is associated with Product" });
        //    }
        //}
        //#endregion
    }
}
