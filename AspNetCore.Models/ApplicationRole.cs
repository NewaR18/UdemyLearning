using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models
{
    public class ApplicationRole : IdentityRole
    {
        [DisplayName("Menus")]
        public string ListOfMenuId { get; set; }
    }
}
