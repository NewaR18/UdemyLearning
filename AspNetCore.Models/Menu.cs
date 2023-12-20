using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models
{
    public class Menu
    {
        [Key]
        [DisplayName("Menu Code")]
        public int MenuId { get; set; }
        [DisplayName("Menu Name")]
        public string Name { get; set; }
    }
}
