using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Models.JSModels
{
	public class LinkModel
	{
		public string? ControllerName { get; set; }
		public string? ActionName { get; set; }
		public string? AreaName { get; set; }
		public string? QueryId { get; set; }
	}
}
