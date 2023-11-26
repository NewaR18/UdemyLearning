using AspNetCore.Models;
using AspNetCoreFromBasic.Repository;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreFromBasic.Controllers
{
    public class LibraryController : Controller
    {
        private readonly ILibraryRepo _repo;
        public LibraryController(ILibraryRepo repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            IEnumerable<Library> books = _repo.Index();
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
                _repo.Create(lib);
                TempData["success"] = "New Item Created Successfully";
                return RedirectToAction("Index");
            }
            return View();
            //return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Library lib = _repo.GetById(id);
            return View(lib);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            Library lib = _repo.GetById(id);
            return View(lib);
        }
        [HttpPost]
        public IActionResult Edit(Library lib)
        {
            _repo.Update(lib);
            TempData["success"] = "Item Updated Successfully";
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            _repo.Delete(id);
            TempData["success"] = "Item Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
