using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreFromBasic.Areas.Admin.Controllers
{
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _repo;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _repo = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Company entity)
        {

            if (ModelState.IsValid)
            {
                _repo.CompanyRepo.Add(entity);
                _repo.Save();
                TempData["success"] = "Company Created Successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Company could not be created !! Validation error";
            return View();
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            Company entity = _repo.CompanyRepo.GetFirstOrDefault(x => x.Id == id);
            return View(entity);
        }
        public IActionResult Edit(int id)
        {
            Company entity = _repo.CompanyRepo.GetFirstOrDefault(x => x.Id == id);
            return View(entity);
        }
        [HttpPost]
        public IActionResult Edit(Company entity)
        {
            _repo.CompanyRepo.Update(entity);
            _repo.Save();
            TempData["success"] = "Company Updated Successfully";
            return RedirectToAction("Index");
        }
        #region API
        [HttpGet]
        public JsonResult GetAll()
        {
            IEnumerable<Company> entities = _repo.CompanyRepo.GetAll();
            return Json(new { data = entities });
        }
        public IActionResult Delete(int id)
        {
            Company company = _repo.CompanyRepo.GetFirstOrDefault(x => x.Id == id);
            if (company == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            else
            {
                _repo.CompanyRepo.Remove(company);
                _repo.Save();
                return Json(new { success = true, message = "Deleted Successfully" });
            }
        }
        #endregion
    }
}
