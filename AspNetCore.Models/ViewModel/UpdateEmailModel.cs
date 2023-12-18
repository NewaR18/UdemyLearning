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
    public class UpdateEmailModel
    {
        [ValidateNever]
        public string Email { get; set; }
        [ValidateNever]
        public bool IsEmailConfirmed { get; set; }
        [BindProperty]
        public UpdateEmailInputModel Input { get; set; }
    }
    public class UpdateEmailInputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }
    }
}
