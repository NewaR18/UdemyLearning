using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Utilities.Commons
{
	public class SMSSending
	{
		private readonly IConfiguration _configuration;
		public SMSSending(IConfiguration configuration) 
		{ 
			_configuration = configuration;
		}
		public void SendSMS(string destinationNumber, string messageBody)
		{
			
		}
	}
}
