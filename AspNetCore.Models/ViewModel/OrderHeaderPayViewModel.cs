using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models.ViewModel
{
	public class OrderHeaderPayViewModel
	{
		public OrderHeader OrderHeader { get; set; }	
		public string PaymentMethod { get; set; }
	}
}
