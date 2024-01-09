using AspNetCore.DataAccess.Repository;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using AspNetCore.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreFromBasic.Areas.Admin.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CustomersController(UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Create(RegisterModel registerModel)
        {
            ModelState.Remove("Input.ConfirmPassword");
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.UserName = registerModel.Input.Email;
                user.Email = registerModel.Input.Email;
                user.PhoneNumber = registerModel.Input.PhoneNumber;
                user.Name = registerModel.Input.Name;
                user.Gender = registerModel.Input.Gender;
                user.Address = registerModel.Input.Address;
                var result = await _userManager.CreateAsync(user, registerModel.Input.Password);
                if (result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var resultConfirmation = await _userManager.ConfirmEmailAsync(user, code);
                    if (resultConfirmation.Succeeded)
                    {
                        TempData["success"] = "User Created Successfully";
                        return RedirectToAction("Index");
                    }
                    TempData["success"] = "User created but could not confirm email";
                    return RedirectToAction(nameof(Index));   
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }
        public async Task<IActionResult> Details(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            ProfileIndexModel profileData = new ProfileIndexModel()
            {
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                ImageURL = user.ImageURL
            };
            return View(profileData);
        }
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            ProfileIndexModel profileData = new ProfileIndexModel()
            {
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                ImageURL = user.ImageURL
            };
            return View(profileData);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ProfileIndexModel profileData, IFormFile? file)
        {
            var user = await _userManager.FindByEmailAsync(profileData.Email);
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string FileName = Guid.NewGuid().ToString();
                    var FilePath = Path.Combine(wwwRootPath, "images/users");
                    var FileExtension = Path.GetExtension(file.FileName);
                    var FinalPath = Path.Combine(FilePath, FileName + FileExtension);
                    if (profileData.ImageURL != null)
                    {
                        var FileToBeDeleted = Path.Combine(wwwRootPath, profileData.ImageURL.TrimStart('/'));
                        if (System.IO.File.Exists(FileToBeDeleted))
                        {
                            System.IO.File.Delete(FileToBeDeleted);
                        }
                    }
                    using (var fileStream = new FileStream(FinalPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    user.ImageURL = @"/images/users/" + FileName + FileExtension;
                    profileData.ImageURL = user.ImageURL;
                }
                user.Name = profileData.Name;
                user.Address = profileData.Address;
                user.Gender = profileData.Gender;
                user.PhoneNumber = profileData.PhoneNumber;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["success"] = "User Updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View();
        }
        #region API CALL
        [HttpGet]
        public JsonResult GetAll()
        {
            var entities = _userManager.Users.Select(user =>
                new
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Address = user.Address,
                    Gender = user.Gender == 'M' ? "Male" : (user.Gender == 'F' ? "Female" : "Unknown"),
                    PhoneNumber = user.PhoneNumber
                });
            return Json(new { data = entities });
        }
        [HttpDelete]
        public async Task<JsonResult> Delete(string id)
        {
            var user =  await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            else
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                var FileToBeDeleted = Path.Combine(wwwRootPath, user.ImageURL.TrimStart('/'));
                if (System.IO.File.Exists(FileToBeDeleted))
                {
                    System.IO.File.Delete(FileToBeDeleted);
                }
                var result= await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "Deleted Successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Error while deleting" });
                }
            }
        }
        #endregion

        #region Functions
        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Admin/Account/Register.cshtml");
            }
        }
        #endregion
    }
}
