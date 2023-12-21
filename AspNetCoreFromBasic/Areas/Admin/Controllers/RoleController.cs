using AspNetCore.DataAccess.Data;
using AspNetCore.DataAccess.Repository;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using AspNetCore.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Transactions;

namespace AspNetCoreFromBasic.Areas.Admin.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public RoleController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            IEnumerable<ApplicationRole> data=_roleManager.Roles;
            return View(data);
        }
        public IActionResult Create()
        {
            RoleModel roleModel = new RoleModel()
            {
                AvailableMenus = _unitOfWork.MenuRepo.GetAll().Select(x => new Menu()
                {
                    MenuId = x.MenuId,
                    Name = x.Name
                }).ToList(),
            };
            return View(roleModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleModel entity)
        {
            if(ModelState.IsValid)
            {
                ApplicationRole role=new ApplicationRole();
                role.Name = entity.Name;
                if (entity.SelectedMenuIds.Any())
                {
                    role.ListOfMenuId = string.Join(',',entity.SelectedMenuIds);
                }
                var result=await _roleManager.CreateAsync(role);
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
            ApplicationRole role= await _roleManager.FindByIdAsync(id);
            IEnumerable<string> result = role.ListOfMenuId.Split(',').Select(item => item.Trim());
            RoleModel roleModel = new RoleModel()
            {
                Name = role.Name,
                SelectedMenuIds = result.ToList(),
                AvailableMenus = _unitOfWork.MenuRepo.GetAll().Select(x => new Menu()
                {
                    MenuId = x.MenuId,
                    Name = x.Name
                }).ToList(),
            };
            return View(roleModel);
        }
        public async Task<IActionResult> Edit(string id)
        {
            ApplicationRole role = await _roleManager.FindByIdAsync(id);
            IEnumerable<string> result = new List<string>();
            if (!string.IsNullOrEmpty(role.ListOfMenuId))
            {
                result = role.ListOfMenuId.Split(',').Select(item => item.Trim());
            }
            
            RoleModel roleModel = new RoleModel()
            {
                Name = role.Name,
                Role = role,
                SelectedMenuIds = result.ToList(),
                AvailableMenus = _unitOfWork.MenuRepo.GetAll().Select(x => new Menu()
                {
                    MenuId = x.MenuId,
                    Name = x.Name
                }).ToList(),
            };
            return View(roleModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(RoleModel roleModel)
        {
            if (ModelState.IsValid)
            {
                ApplicationRole role = await _roleManager.FindByIdAsync(roleModel.Role.Id);
                role.Name = roleModel.Name;
                role.ListOfMenuId = "";
                if (roleModel.SelectedMenuIds!=null)
                {
                    role.ListOfMenuId = string.Join(',', roleModel.SelectedMenuIds);
                }
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    TempData["success"] = "Role Updated";
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
            return View(roleModel);
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
                    ApplicationRole Role = await _roleManager.FindByIdAsync(item);
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
                    return RedirectToAction("RoleAssignmentIndex");
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
                ApplicationRole identityrole = await _roleManager.FindByNameAsync(role);
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
                    ApplicationRole Role = await _roleManager.FindByIdAsync(item);
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
                                return RedirectToAction("RoleAssignmentIndex");
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
            ApplicationRole role = await _roleManager.FindByIdAsync(id);
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
