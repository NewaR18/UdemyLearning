using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Utilities.ApiGateway
{
	public static class APIGateway
	{
		public static string EsewaSuccess = BaseURL.GetBaseURL()+ "Customer/Cart/EsewaSuccess";
		public static string EsewaFailure = BaseURL.GetBaseURL()+ "Customer/Cart/EsewaFailure";
		public static string StripeSuccess = BaseURL.GetBaseURL()+ "Customer/Cart/StripeSuccess";
		public static string StripeFailure = BaseURL.GetBaseURL()+ "Customer/Cart/StripeFailure";
		public static string KhaltiSuccess = BaseURL.GetBaseURL()+ "Customer/Cart/KhaltiSuccess";
		public static string KhaltiFailure = BaseURL.GetBaseURL()+ "Customer/Cart/KhaltiFailure";
	}
}
