using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AspNetCore.Utilities.Middleware
{
    public class AuthorizationMiddleware : IMiddleware
    {
        
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IUnitOfWork _repo;
        public AuthorizationMiddleware(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IUnitOfWork repo)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
            _repo = repo;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            ClaimsPrincipal user = _httpContextAccessor.HttpContext?.User!;
            if (user != null)
            {
                IEnumerable<Claim> roleClaims = user.FindAll(ClaimTypes.Role);
                if (roleClaims.Any())
                {
                    //Find Role Name By User
                    IEnumerable<string> userRoleNames = roleClaims.Select(c => c.Value);
                    List<string> menuIds = new List<string>();
                    //Find List of MenuId By User
                    foreach (var userRoleName in userRoleNames)
                    {
                        ApplicationRole applicationRole = await _roleManager.FindByNameAsync(userRoleName);
                        if(applicationRole.ListOfMenuId != null)
                        {
                            menuIds.Add(applicationRole.ListOfMenuId);
                        }
                    }
                    string menuIdsCombined = string.Join(",", menuIds);
                    IEnumerable<string> menuIdsCombinedEnumerable = menuIdsCombined.Split(',');
                    IEnumerable<string> menuIdsCombinedDistinct = menuIdsCombinedEnumerable.Distinct();
                    IEnumerable<string> menusNames = _repo.MenuRepo.GetAll().Where(menu => menuIdsCombinedDistinct.Any(x=>x.Equals(menu.MenuId.ToString()))).Select(menu => menu.Name);
                    var controllerName = context.GetRouteValue("controller")?.ToString();
                    if (menusNames.Any(x => x.Equals(controllerName)) || IsAccessibleForAnyUser(controllerName))
                    {
                        await next(context);
                    }
                    else if(context.Request.Path.Value == "/dashboardHub" || context.Request.Path.Value == "/chatHub")
                    {
                        await next(context);
                    }
                    else
                    {
                        context.Response.Redirect("/Customer/Home/AccessDenied");
                    }
                }
                else
                {
                    var controllerName = context.GetRouteValue("controller")?.ToString();
                    if(IsAccessibleForAnyUser(controllerName))
                    {
                        await next(context);
                    }
                    else
                    {
                        context.Response.Redirect("/Customer/Home/AccessDenied");
                    }
                }
            }
            else
            {
                var controllerName = context.GetRouteValue("controller")?.ToString();
                if (IsAccessibleForAnyUser(controllerName))
                {
                    await next(context);
                }
                else
                {
                    context.Response.Redirect("/Customer/Home/AccessDenied");
                }
            }

        }
        public bool IsAccessibleForAnyUser(string controllerName)
        {
            if (controllerName == "Home" || controllerName == "Account")
            {
                return true;
            }
            return false;
        }
    }
}
