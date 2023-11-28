using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreFromBasic.Controllers
{
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _repo;
        public CoverTypeController(IUnitOfWork repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            IEnumerable<CoverType> entities= _repo.CoverTypeRepo.GetAll();
            return View(entities);
        }
        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CoverType entity)
        {
            if (ModelState.IsValid)
            {
                _repo.CoverTypeRepo.Add(entity);
                _repo.Save();
                TempData["success"] = "Item Created Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Item could not be created !! Validation error";
            return View();
        }
        public IActionResult Edit(int id)
        {
            CoverType entity = _repo.CoverTypeRepo.GetFirstOrDefault(x => x.Id == id);
            return View(entity);
        }
        [HttpPost]
        public IActionResult Edit(CoverType entity)
        {
            if (ModelState.IsValid)
            {
                _repo.CoverTypeRepo.Update(entity);
                _repo.Save();
                TempData["success"] = "Item Edited Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Item could not be edited !! Validation error";
            return View(entity);
        }
        public IActionResult Details(int id)
        {
            CoverType entity = _repo.CoverTypeRepo.GetFirstOrDefault(x => x.Id == id);
            return View(entity);
        }
        public IActionResult Delete(int id)
        {
            CoverType entity = _repo.CoverTypeRepo.GetFirstOrDefault(x => x.Id == id);
            _repo.CoverTypeRepo.Remove(entity);
            _repo.Save();
            TempData["success"] = "Item Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
