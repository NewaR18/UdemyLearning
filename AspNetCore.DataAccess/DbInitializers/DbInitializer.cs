using AspNetCore.DataAccess.Data;
using AspNetCore.Models;
using AspNetCore.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.DataAccess.DbInitializers
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AppDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public DbInitializer(AppDbContext context, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            //Apply Pending Migration
            try
            {
                if (_context.Database.GetPendingMigrations().Count()>0)
                {
                    _context.Database.Migrate();
                }
            }catch(Exception ex)
            {

            }

            //Add Controller to Menu if it is already not present
            try
            {
                AddNewControllersToMenu();
            }
            catch (Exception ex)
            {

            }
        }
        private static IEnumerable<string> GetControllerNames()
        {
            var assembly = Assembly.Load("AspNetCoreFromBasic");
            var controllerTypes = assembly
                                           .GetTypes()
                                           .Where(type => typeof(Controller).IsAssignableFrom(type) && type.Name.EndsWith("Controller"));
            var controllerNames = controllerTypes.Select(type => type.Name.Substring(0, type.Name.Length - "Controller".Length));
            return controllerNames;
        }
        private void AddNewControllersToMenu()
        {
            var currentControllers = _context.Menu.Select(menu => menu.Name).ToList();
            IEnumerable<string> controllerNames = GetControllerNames();
            var newControllers = controllerNames.Except(currentControllers);
            var newMenus = newControllers.Select(controllerName => new Menu
            {
                MenuId = 0,
                Name = controllerName
            });
            _context.Menu.AddRange(newMenus);
            _context.SaveChanges();
            foreach (var newController in newControllers)
            {
                GiveNewControllerPermissionToAdmin(newController).GetAwaiter().GetResult();
            }
        }
        private async Task GiveNewControllerPermissionToAdmin(string newController)
        {
            ApplicationRole role = await _roleManager.FindByNameAsync("Admin");
            Menu menu = _context.Menu.Where(menu => menu.Name == newController).FirstOrDefault()!;
            if(menu!=null)
            {
                role.ListOfMenuId = string.Concat(role.ListOfMenuId, ',', menu.MenuId);
                var result = await _roleManager.UpdateAsync(role);
            }
        }
    }
}
