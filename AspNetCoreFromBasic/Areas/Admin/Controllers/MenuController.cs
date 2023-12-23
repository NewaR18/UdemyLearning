using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreFromBasic.Areas.Admin.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly IUnitOfWork _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public MenuController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _repo = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
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
        #region API
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return Json(new { success = false, message = "Error" });
            }
            else
            {
                var roles = _roleManager.Roles.ToList().Where(x => x.ListOfMenuId.Split(',').ToList().Contains(id.ToString()));
                foreach(var role in roles)
                {
                    List<string> roleMenus = role.ListOfMenuId.Split(',').ToList();
                    if (roleMenus.Exists(x => x.Equals(id.ToString())))
                    {
                        roleMenus.Remove(id.ToString());
                    }
                    role.ListOfMenuId = string.Join(",", roleMenus);
                    var result = await _roleManager.UpdateAsync(role);
                }
                Menu entityMenu = _repo.MenuRepo.GetFirstOrDefault(x => x.MenuId.Equals(id));
                _repo.MenuRepo.Remove(entityMenu);
                _repo.Save();
                return Json(new { success = true, message = "Menu Deleted Successfully" });
            }
        }
        #endregion
    }
}
