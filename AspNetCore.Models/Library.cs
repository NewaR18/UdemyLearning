using System.ComponentModel.DataAnnotations;

namespace AspNetCore.Models
{
    public class Library
    {
        [Key]
        public int id { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "Why insert more than that")]
        public string Name { get; set; }
        [Required]
        [StringLength(15, ErrorMessage = "MSDN cannot be of size more than 15")]
        public string MSDN { get; set; }
        [Required]
        [Display(Name = "Total Page")]
        public int Pages { get; set; }
    }
}
