using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models.ViewModel
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCart> ListCart { get; set; }
    }
}
