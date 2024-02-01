using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Utilities.Enumerators
{
	public enum PaymentEnum
	{
		Pending = 0,
		Approved = 1,
		ApprovedForDelayedPayment = 2,
		Rejected = 3
	}
}
