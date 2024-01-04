using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models
{
    public class EsewaPayment
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string? TransactionId { get; set; }
        public string? TransactionCode { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string? ProductCode { get; set; }
        public DateTime? TransactionDate { get; set; } = DateTime.Now;
        public int ProductId { get; set; }
        [ValidateNever]
        public Product Product { get; set; }
        public int Count { get; set; }
    }
}
