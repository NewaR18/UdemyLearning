using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Utilities.Middleware
{
    public static class SyncControllerAndMenu
    {
        public static void RunWithProgramStart(this IApplicationBuilder app)
        {
            //var controllers=GetControllerNames();
            //Logic to sync Controller with Menu Database
        }
        //private static IEnumerable<string> GetControllerNames()
        //{
        //    var controllerTypes = Assembly.GetExecutingAssembly()
        //                                   .GetTypes()
        //                                   .Where(type => typeof(Controller).IsAssignableFrom(type) && type.Name.EndsWith("Controller"));

        //    //var controllerNames = controllerTypes.Select(type => type.Name.Substring(0, type.Name.Length - "Controller".Length));
        //    return controllerNames;
        //}
    }
    

}
