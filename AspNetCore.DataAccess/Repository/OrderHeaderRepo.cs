using AspNetCore.CommonFunctions.Expressions;
using AspNetCore.DataAccess.Data;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using AspNetCore.Models.JSModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.DataAccess.Repository
{
    public class OrderHeaderRepo : Repo<OrderHeader>, IOrderHeaderRepo 
	{
        private readonly AppDbContext _context;
        
        public OrderHeaderRepo(AppDbContext context) :base(context)
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
                if(paymentStatus == "Approved")
                {
                    orderHeader.PaymentDate = DateTime.Now;
                }
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
        public new IEnumerable<OrderHeader> GetAll(Expression<Func<OrderHeader, bool>>? filter = null, string? IncludeProperties = null)
		{
            IQueryable<OrderHeader> query = _context.OrderHeader;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (IncludeProperties != null)
            {
                foreach (var property in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query;
        }
        public PaginatedOrderHeader GetPaginatedRows(int skip, int TotalCountPerPage, List<ColumnFilterModel> Columns)
        {
            PredicateFilter _predicate = new PredicateFilter();
            skip = skip < 0 ? 0 : skip;
            PaginatedOrderHeader paginatedOrderHeader = new PaginatedOrderHeader()
            {
                Data = _context.OrderHeader
                                            .Where(_predicate.predicate(Columns))
                                            .Skip(skip).Take(TotalCountPerPage).AsNoTracking(),
                TotalCount = _context.OrderHeader.Where(_predicate.predicate(Columns)).Count()
            };
            return paginatedOrderHeader;
        }
    }
}
