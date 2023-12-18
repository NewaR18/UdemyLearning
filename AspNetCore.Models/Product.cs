using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public decimal ListPrice { get; set; }
        [Required]
        [Range(0,100000)]
        [DisplayName("Price for 0-50")]
        public decimal Price { get; set; }
        [Required]
        [Range(0, 100000)]
        [DisplayName("Price for 51-100")]
        public decimal Price50 { get; set;}
        [Required]
        [Range(0, 100000)]
        [DisplayName("Price for 100+")]
        public decimal Price100 { get; set;}
        [ValidateNever]
        [DisplayName("Image")]
        public string ImageURL { get; set; }
        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
        [Required]
        [DisplayName("Cover Type")]
        public int CoverTypeId { get; set; }
        [ValidateNever]
        public CoverType CoverType { get; set; }
    }   
}
