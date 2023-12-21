using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models.ViewModel
{
    public class RoleModel
    {
        [Required]
        public string Name { get; set; }
        [ValidateNever]
        public ApplicationRole Role { get; set; }
        [ValidateNever]
        public List<string> SelectedMenuIds { get; set; }
        [ValidateNever]
        public List<Menu> AvailableMenus { get; set; }
    }
}
