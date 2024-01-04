using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models.ViewModel
{
    public class EsewaSuccessResponse
    {
        public string status { get; set; }
        public string signature { get; set; }
        public string transaction_code { get; set; }
        public decimal total_amount { get; set; }
        public string transaction_uuid { get; set; }
        public string product_code { get; set; }
        public string success_url { get; set; }
        public string signed_field_names { get; set; }
    }
}
