using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models
{
    public class ShoppingCart
    {
        public Product Product { get; set; }
        public int Count { get; set; }
    }
}
