using AspNetCore.DataAccess.Data;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.DataAccess.Repository;
using AspNetCore.Utilities.EmailConfigurations;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Utilities.Middleware;

namespace AspNetCore.Utilities.Modules
{
    public static class ServiceModule
    {
        public static void RegisterModule(this IServiceCollection services)
        {
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<AuthorizationMiddleware>();
            services.AddScoped<AppDbContext>();
        }
    }
}
