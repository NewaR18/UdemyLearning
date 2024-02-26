using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models
{
    public class PaymentRefund
    {
        [Key]
        public int Id { get; set; }
        public string PaymentOptions { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string PaymentIntentId { get; set; }
        [ValidateNever]
        public string? StripeId { get; set; }
        [ValidateNever]
        public string? BalanceTransactionId { get; set; }
        [ValidateNever]
        public string? ChargeId { get; set; }
        [ValidateNever]
        public string? RequestId { get; set; }
    }
}
