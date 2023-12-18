using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models.ViewModel
{
    public class ExternalLoginModel
    {
        [BindProperty]
        public ExternalLoginInput Input { get; set; }
        [ValidateNever]
        public string ProviderDisplayName { get; set; }
    }
    public class ExternalLoginInput
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
