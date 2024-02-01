using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using AspNetCore.Models.CustomDataAnnotation;
using AspNetCore.Models.Enumerators;

namespace AspNetCore.Models.ViewModel
{
    public class ShoppingCartViewModel
    {
        [ValidateNever]
        public IEnumerable<ShoppingCart> ListCart { get; set; }
        public OrderHeader OrderHeader { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
    }
}
