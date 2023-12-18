using AspNetCore.DataAccess.Repository;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreFromBasic.Areas.Admin.Controllers
{
    [Authorize]
    public class LibraryController : Controller
    {
        private readonly IUnitOfWork _repo;
        public LibraryController(IUnitOfWork repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            IEnumerable<Library> books = _repo.LibraryRepo.GetAll();
            return View(books);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Library lib)
        {
            if (lib.Name == lib.MSDN)
            {
                ModelState.AddModelError("Name", "Name cannot be same to MSDN");
            }
            if (ModelState.IsValid)
            {
                _repo.LibraryRepo.Add(lib);
                _repo.Save();
                TempData["success"] = "New Item Created Successfully";
                return RedirectToAction("Index");
            }
            return View();
            //return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Library lib = _repo.LibraryRepo.GetFirstOrDefault(x => x.id == id);
            return View(lib);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            Library lib = _repo.LibraryRepo.GetFirstOrDefault(x => x.id == id);
            return View(lib);
        }
        [HttpPost]
        public IActionResult Edit(Library lib)
        {
            _repo.LibraryRepo.Update(lib);
            _repo.Save();
            TempData["success"] = "Item Updated Successfully";
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            Library entity = _repo.LibraryRepo.GetFirstOrDefault(x => x.id == id);
            _repo.LibraryRepo.Remove(entity);
            _repo.Save();
            TempData["success"] = "Item Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
