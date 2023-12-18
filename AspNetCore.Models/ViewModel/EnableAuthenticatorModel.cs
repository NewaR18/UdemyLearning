using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AspNetCore.Models.ViewModel
{
    public class EnableAuthenticatorModel
    {
        [ValidateNever]
        public string SharedKey { get; set; }
        [ValidateNever]
        public string AuthenticatorUri { get; set; }
        [ValidateNever]
        public string[] RecoveryCodes { get; set; }
        [BindProperty]
        public EnableAuthenticatorInputModel Input { get; set; }
    }
    public class EnableAuthenticatorInputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }
    }
}
