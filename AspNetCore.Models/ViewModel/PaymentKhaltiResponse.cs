using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models.ViewModel
{
    public class PaymentKhaltiResponse
    {
        public string pidx { get; set; }
        public string tidx { get; set; }
        public string status { get; set; }
        public int amount { get; set; }
        public string mobile { get; set; }
        public string purchase_order_id { get; set; }
        public string purchase_order_name { get; set; }
    }
   
}
