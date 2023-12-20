using AspNetCore.DataAccess.Data;
using AspNetCore.Models;
using AspNetCore.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace AspNetCoreFromBasic.Areas.Admin.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        public RoleController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _roleManager = roleManager;
            _userManager= userManager;
            _context= context;
        }
        public IActionResult Index()
        {
            IEnumerable<IdentityRole> data=_roleManager.Roles;
            return View(data);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleModel entity)
        {
            if(ModelState.IsValid)
            {
                var result=await _roleManager.CreateAsync(new IdentityRole(entity.Name));
                if(result.Succeeded)
                {
                    TempData["success"] = "New Role Created";
                    return RedirectToAction("Index");
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
        public async Task<IActionResult> Details(string id)
        {
            IdentityRole role= await _roleManager.FindByIdAsync(id);
            return View(role);
        }
        public async Task<IActionResult> Edit(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            return View(role);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                role.NormalizedName = role.Name.ToUpper();
                var result= await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    TempData["success"] = "Role Edited Successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            IdentityRole roleDisplay = await _roleManager.FindByIdAsync(role.Id);
            return View(roleDisplay);
        }
        public IActionResult RoleAssignment()
        {
            RoleAssignmentViewModel roleAssignmentViewModel = new RoleAssignmentViewModel()
            {
                UserList = _userManager.Users.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id
                }),
                RoleList = _roleManager.Roles.Select(x => new SelectListItem
                {
                    Text= x.Name,
                    Value = x.Id
                })

            };
            return View(roleAssignmentViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> RoleAssignment(RoleAssignmentViewModel roleAssignmentViewModel)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(roleAssignmentViewModel.ApplicationUserId);
                List<string> Roles = new List<string>();
                foreach (var item in roleAssignmentViewModel.IdentityRoleId)
                {
                    IdentityRole Role = await _roleManager.FindByIdAsync(item);
                    if (Role != null)
                    {
                        Roles.Add(Role.Name);
                    }
                }
                IEnumerable<string> RoleListEnumerable= Roles;
                var result = await _userManager.AddToRolesAsync(user, RoleListEnumerable);
                if(result.Succeeded)
                {
                    TempData["success"] = $"Roles Assigned to {user.Name}";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            roleAssignmentViewModel.UserList = _userManager.Users.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            });
            roleAssignmentViewModel.RoleList = _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            });
            return View(roleAssignmentViewModel);
        }
        public IActionResult RoleAssignmentIndex()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> RoleAssignmentDetails(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            UserRole userRole = new UserRole();
            IEnumerable<string> roles = await _userManager.GetRolesAsync(user);
            string rolesAsString = string.Join(", ", roles);
            userRole.UserId = user.Id;
            userRole.UserName = user.Name;
            userRole.RoleName = rolesAsString;
            return View(userRole);
        }
        [HttpGet]
        public async Task<IActionResult> RoleAssignmentUpdate(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            IEnumerable<string> identityRolesList = await _userManager.GetRolesAsync(user);
            List<string> identityRolesIdList = new List<string>();
            foreach (var role in identityRolesList)
            {
                IdentityRole identityrole = await _roleManager.FindByNameAsync(role);
                var RoleId = await _roleManager.GetRoleIdAsync(identityrole);
                identityRolesIdList.Add(RoleId);
            }
            string[] identityRolesArray = identityRolesIdList.ToArray();
            RoleAssignmentViewModel roleAssignmentViewModel = new RoleAssignmentViewModel()
            {
                ApplicationUserId = userId,
                IdentityRoleId = identityRolesArray,
                UserList = _userManager.Users.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id
                }),
                RoleList = _roleManager.Roles.Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id
                })

            };
            return View(roleAssignmentViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> RoleAssignmentUpdate(RoleAssignmentViewModel roleAssignmentViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(roleAssignmentViewModel.ApplicationUserId);
                List<string> Roles = new List<string>();
                foreach (var item in roleAssignmentViewModel.IdentityRoleId)
                {
                    IdentityRole Role = await _roleManager.FindByIdAsync(item);
                    if (Role != null)
                    {
                        Roles.Add(Role.Name);
                    }
                }
                IEnumerable<string> RoleListEnumerable = Roles;

                using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        IEnumerable<string> identityRolesList = await _userManager.GetRolesAsync(user);
                        if (identityRolesList.Any())
                        {
                            var resultDeletion = await _userManager.RemoveFromRolesAsync(user, identityRolesList);
                            if (resultDeletion.Succeeded)
                            {
                                var resultAddition = await _userManager.AddToRolesAsync(user, RoleListEnumerable);
                                if (resultAddition.Succeeded)
                                {
                                    scope.Complete();
                                    TempData["success"] = $"Roles Assigned to {user.Name}";
                                    return RedirectToAction("Index");
                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }
                            else
                            {
                                foreach (var error in resultDeletion.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                            }
                        }
                        else
                        {
                            var resultAddition = await _userManager.AddToRolesAsync(user, RoleListEnumerable);
                            if (resultAddition.Succeeded)
                            {
                                scope.Complete();
                                TempData["success"] = $"Roles Assigned to {user.Name}";
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                foreach (var error in resultAddition.Errors)
                                {
                                    ModelState.AddModelError(string.Empty, error.Description);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        TempData["error"] = ex.Message;
                    }

                }
            }
            roleAssignmentViewModel.UserList = _userManager.Users.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            });
            roleAssignmentViewModel.RoleList = _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            });
            return View(roleAssignmentViewModel);
        }

        #region API
        public async Task<IActionResult> RoleAssignmentDelete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            IEnumerable<string> identityRolesList = await _userManager.GetRolesAsync(user);
            if (identityRolesList.Any())
            {
                var resultDeletion = await _userManager.RemoveFromRolesAsync(user, identityRolesList);
                if (resultDeletion.Succeeded)
                {
                    return Json(new { success = true, message = "Deleted Successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Error while deleting" });
                }
            }
            else
            {
                return Json(new { success = false, message = "No Roles To Delete. If you want to Delete Customer, Go to Customers Section" });
            }
        }
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            else
            {
                var result= await _roleManager.DeleteAsync(role);
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
        [HttpGet]
        public async Task<IActionResult> GetUserRoles()
        {
            var users = await _userManager.Users.ToListAsync();
            List<UserRole> userRoles = new List<UserRole>();
            foreach (var user in users)
            {
                UserRole userRole = new UserRole();
                IEnumerable<string> roles = await _userManager.GetRolesAsync(user);
                string rolesAsString = string.Join(", ", roles);
                userRole.UserId=user.Id;
                userRole.UserName = user.Name;
                userRole.RoleName = rolesAsString;
                userRoles.Add(userRole);
            }
            return Json(new { data = userRoles });
        }
        #endregion
    }
}
