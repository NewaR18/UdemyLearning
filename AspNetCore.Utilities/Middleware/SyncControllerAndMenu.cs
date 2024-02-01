 using AspNetCore.DataAccess.Data;
using AspNetCore.DataAccess.Repository;
using AspNetCore.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Utilities.Middleware
{
    public static class SyncControllerAndMenu
    {
        //This way you can invoke method when running program for the fiest time. you have to include app.RunWithProgramStart(); in program.cs
        public static void RunWithProgramStart(this IApplicationBuilder app)
        {
            //var controllers = GetControllerNames();
        }

        //Get Controller Names
        /*private static IEnumerable<string> GetControllerNames()
        {
            var assembly = Assembly.Load("AspNetCoreFromBasic");
            var controllerTypes = assembly
                                           .GetTypes()
                                           .Where(type => typeof(Controller).IsAssignableFrom(type) && type.Name.EndsWith("Controller"));
            var controllerNames = controllerTypes.Select(type => type.Name.Substring(0, type.Name.Length - "Controller".Length));
            return controllerNames;
        }*/
    }


}
