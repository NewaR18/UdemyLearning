using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models.ViewModel
{
    public class PaymentGatewayParam
    {
        public string Path { get; set; }
        public PaymentGatewayParamInner Params { get; set; }
    }
    public class PaymentGatewayParamInner
    {
        public string amount { get; set; }
        public string tax_amount { get; set; }
        public string total_amount { get; set; }
        public string transaction_uuid { get; set; }
        public string product_code { get; set; }
        public string product_service_charge { get; set; }
        public string product_delivery_charge { get; set; }
        public string success_url { get; set; }
        public string failure_url { get; set; }
        public string signed_field_names { get; set; }
        public string signature { get; set; }
    }
}
