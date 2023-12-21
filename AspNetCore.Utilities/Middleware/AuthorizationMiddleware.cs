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

namespace AspNetCore.Utilities.Middleware
{
    public class AuthorizationMiddleware
    {
        
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RequestDelegate _next;
        public AuthorizationMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task Invoke(HttpContext context)
        {
            ClaimsPrincipal user = _httpContextAccessor.HttpContext?.User!;
            if (user != null)
            {
                IEnumerable<Claim> roleClaims = user.FindAll(ClaimTypes.Role);
                if (roleClaims.Any())
                {
                    IEnumerable<string> userRoles = roleClaims.Select(c => c.Value);
                    string userRolesInString = string.Join(',', userRoles);
                    List<string> menuNames = new List<string>();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "FindMenuIds";
                        using (SqlConnection conn1 = new SqlConnection("server=192.168.20.31; database=udemy; uid=sudip; pwd=infodev; Trusted_Connection=true; TrustServerCertificate=True"))
                        {
                            cmd.Connection = conn1;
                            conn1.Open();
                            cmd.CommandTimeout = 30;
                            cmd.Parameters.AddWithValue("@roles", userRolesInString);
                            using (SqlDataReader sd = cmd.ExecuteReader())
                            {
                                if (sd.HasRows)
                                {
                                    while (sd.Read())
                                    {
                                        string menuName = sd.GetString(0);
                                        menuNames.Add(menuName);
                                    }
                                }
                            }
                        }
                    }
                    var controllerName = context.GetRouteValue("controller")?.ToString();
                    if (menuNames.Exists(x => x.Equals(controllerName)) || controllerName == "Home" || controllerName == "Account")
                    {
                        await _next(context);
                    }
                    else
                    {
                        context.Response.Redirect("/Customer/Home/Index");
                    }
                }
                else
                {
                    var controllerName = context.GetRouteValue("controller")?.ToString();
                    if(controllerName == "Home" || controllerName == "Account")
                    {
                        await _next(context);
                    }
                    else
                    {
                        context.Response.Redirect("/Customer/Home/Index");
                    }
                }
            }
            else
            {
                var controllerName = context.GetRouteValue("controller")?.ToString();
                if (controllerName == "Home" || controllerName == "Account")
                {
                    await _next(context);
                }
                else
                {
                    context.Response.Redirect("/Customer/Home/Index");
                }
            }

        }
    }
}
