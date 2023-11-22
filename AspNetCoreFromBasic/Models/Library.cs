using System.ComponentModel.DataAnnotations;

namespace AspNetCoreFromBasic.Models
{
    public class Library
    {
        [Key]
        public int id { get; set; }
        [Required]
        [StringLength(30,ErrorMessage ="Why insert more than that")]
        public string Name { get; set; }
        [Required]
        [StringLength (5,ErrorMessage ="haha")]
        public string MSDN { get; set; }
        [Required]
        [Display(Name="Total Page")]
        public int Pages { get; set; }
    }
}
