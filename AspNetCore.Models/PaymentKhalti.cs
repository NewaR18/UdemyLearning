using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models
{
    public class PaymentKhalti
    {
        [Key]
        public int Id { get; set; }
        public string Pidx { get; set; }
        [ValidateNever]
        [AllowNull]
        public decimal Amount { get; set; }
        [ValidateNever]
        [AllowNull]
        public string Status { get; set; }
        [ValidateNever]
        [AllowNull]
        public string? TxnId { get; set; }
        public string? MobileNo { get; set; }
        public string? PurchaseOrderId { get; set; }
        public string? PurchaseOrderName { get; set; }
    }
}
