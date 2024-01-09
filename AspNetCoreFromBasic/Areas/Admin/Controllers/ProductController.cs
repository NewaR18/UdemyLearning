using AspNetCore.DataAccess.Repository;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using AspNetCore.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Drawing;

namespace AspNetCoreFromBasic.Areas.Admin.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _repo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork repo, IWebHostEnvironment webHostEnvironment)
        {
            _repo = repo;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> books = _repo.ProductRepo.GetAll();
            return View(books);
        }
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            ProductViewModel product = new ProductViewModel()
            {
                Product = new Product(),
                CategoryList = _repo.CategoryRepo.GetAll().Select
                (u =>
                    new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                    }
                ),
                CoverTypeList = _repo.CoverTypeRepo.GetAll().Select
                (u =>
                    new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                    }
                )
            };
            if (id == null || id <= 0)
            {
                return View(product);
            }
            else
            {
                product.Product = _repo.ProductRepo.GetFirstOrDefault(x => x.Id == id);
                return View(product);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel entity,IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string FileName = Guid.NewGuid().ToString();
                    var FilePath = Path.Combine(wwwRootPath, "images/product");
                    var FileExtension = Path.GetExtension(file.FileName);
                    var FinalPath = Path.Combine(FilePath, FileName + FileExtension);
                    if (entity.Product.ImageURL != null)
                    {
                        var FileToBeDeleted = Path.Combine(wwwRootPath, entity.Product.ImageURL.TrimStart('/'));
                        if (System.IO.File.Exists(FileToBeDeleted))
                        {
                            System.IO.File.Delete(FileToBeDeleted);
                        }
                    }
                    using (var fileStream = new FileStream(FinalPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    entity.Product.ImageURL = @"/images/product/" + FileName + FileExtension;
                }
                if (entity.Product.Id <= 0)
                {
                    _repo.ProductRepo.Add(entity.Product);
                    _repo.Save();
                    TempData["success"] = "New Item Created Successfully";
                }
                else
                {
                    _repo.ProductRepo.Update(entity.Product);
                    _repo.Save();
                    TempData["success"] = "Item Updated Successfully";
                }
                return RedirectToAction(nameof(Index));
            }
            return View();
            //return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Product entity = _repo.ProductRepo.GetFirstOrDefault(x => x.Id == id);
            return View(entity);
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            ProductViewModel product = new ProductViewModel()
            {
                Product = _repo.ProductRepo.GetFirstOrDefault(x => x.Id == id),
                CategoryList = _repo.CategoryRepo.GetAll().Select
                (u =>
                    new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                    }
                ),
                CoverTypeList = _repo.CoverTypeRepo.GetAll().Select
                (u =>
                    new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                    }
                )
            };
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product entity)
        {
            _repo.ProductRepo.Update(entity);
            _repo.Save();
            TempData["success"] = "Item Updated Successfully";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            Product entity = _repo.ProductRepo.GetFirstOrDefault(x => x.Id == id);
            _repo.ProductRepo.Remove(entity);
            _repo.Save();
            TempData["success"] = "Item Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        #region API CALL
        [HttpGet]
        public JsonResult GetAll()
        {
            IEnumerable<Product> entities = _repo.ProductRepo.GetAll(IncludeProperties: "Category,CoverType");
            return Json(new { data = entities });
        }
        [HttpDelete]
        public JsonResult Delete(int? id)
        {
            Product entity = _repo.ProductRepo.GetFirstOrDefault(x => x.Id == id);
            if(entity == null)
            {
                return Json(new { success = false, message= "Error while deleting"}) ;
            }
            else
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                var FileToBeDeleted = Path.Combine(wwwRootPath, entity.ImageURL.TrimStart('/'));
                if (System.IO.File.Exists(FileToBeDeleted))
                {
                    System.IO.File.Delete(FileToBeDeleted);
                }
                _repo.ProductRepo.Remove(entity);
                _repo.Save();
                return Json(new { success = true, message = "Deleted Successfully" });
            }
        }
        #endregion
    }
}
