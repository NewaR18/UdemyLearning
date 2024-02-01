using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models.RequiredComponentForModel.StaticDefinitions
{
	public static class StaticLists
	{
		public static IEnumerable<string> Cities = new List<string>()
		{ "Kathmandu", 
			"Pokhara",
			"Chitwan"
		};
	}
}
