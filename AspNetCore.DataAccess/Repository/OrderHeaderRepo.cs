using AspNetCore.DataAccess.Data;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.DataAccess.Repository
{
    public class OrderHeaderRepo : Repo<OrderHeader>, IOrderHeaderRepo
	{
        private readonly AppDbContext _context;
        public OrderHeaderRepo(AppDbContext context):base(context)
        {
            _context = context;
        }

        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeader.Update(orderHeader);
        }
		public void UpdateStatus(int orderId, string orderStatus, string? paymentStatus = null)
		{
			var orderHeader = _context.OrderHeader.FirstOrDefault(s => s.Id.Equals(orderId));
			if (orderHeader != null)
			{
				orderHeader.OrderStatus = orderStatus;
				if (paymentStatus != null)
					orderHeader.PaymentStatus = paymentStatus;
			}
		}
		public void UpdateStripeData(int orderId, string sessionId, string paymentIntentId)
		{
			var orderHeader = _context.OrderHeader.FirstOrDefault(s => s.Id.Equals(orderId))!;
			if (orderHeader != null)
			{
				orderHeader.SessionId = sessionId;
				orderHeader.PaymentIntentId = paymentIntentId;
			}
		}
	}
}
