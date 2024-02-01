using AspNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepo : IRepo<OrderHeader>
    {
        public void Update(OrderHeader orderHeader);
        public void UpdateStatus(int orderId, string orderStatus, string? paymentStatus = null);
        public void UpdateStripeData(int orderId, string sessionId, string paymentIntentId);
	}
}
