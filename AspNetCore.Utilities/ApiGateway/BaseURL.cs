using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Utilities.ApiGateway
{
	public class BaseURL
	{
		public static string GetBaseURL()
		{
			var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
			IConfigurationRoot Configuration = configuration.Build();
			string baseURL = Configuration["BaseURL"];
			return baseURL;
		}
	}
}
