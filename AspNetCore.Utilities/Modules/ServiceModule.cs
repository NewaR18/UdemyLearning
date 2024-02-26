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
using AspNetCore.Utilities.Payments;
using AspNetCore.Utilities.Commons;
using AspNetCore.CommonFunctions.Expressions;
using AspNetCore.Utilities.ManageBackgroundJobs;
using AspNetCore.DataAccess.DbInitializers;

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
            services.AddScoped<EsewaPayments>();
            services.AddScoped<KhaltiPayments>();
			services.AddScoped<StripePayments>();
			services.AddScoped<SMSSending>();
			services.AddScoped<ManageHangfireJobs>();
            services.AddScoped<IDbInitializer,DbInitializer>();
        }
    }
}
