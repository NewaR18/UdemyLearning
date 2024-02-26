using AspNetCore.CommonFunctions.Expressions;
using AspNetCore.CommonFunctions.PredicateBuilderThis;
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
using System.Security.Claims;
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
        public PaginatedOrderHeader GetPaginatedRows(DataTableAjaxModel dataTableAjaxModel, ClaimsPrincipal User)
       {
            PredicateFilter _predicate = new PredicateFilter();
            var searchCondition = PredicateBuilder.True<OrderHeader>();
            bool ShowAll = false;
            if(User.IsInRole("Admin") || User.IsInRole("Super Admin")){
                ShowAll = true;
            }
            if (!ShowAll)
            {
                var userClaimsIdentity = (ClaimsIdentity)User.Identity!;
                var userIdClaim = userClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                searchCondition = searchCondition.And(s=>s.ApplicationUserId.Equals(userIdClaim.Value));
            }
            if (!string.IsNullOrEmpty(dataTableAjaxModel.Status))
            {
                searchCondition = searchCondition.And(p => p.OrderStatus.ToLower().Contains(dataTableAjaxModel.Status.ToLower()));
            }
            //var data2 = _context.OrderHeader
            //                                .Where(_predicate.predicate(dataTableAjaxModel.Columns)).Where(searchCondition)
            //                               .Skip(dataTableAjaxModel.Start).Take(dataTableAjaxModel.Length).AsNoTracking();
            IQueryable<OrderHeader> AllData = _context.OrderHeader
                                            .Where(_predicate.predicate(dataTableAjaxModel.Columns)).Where(searchCondition);
            var paginatedData = AllData.Skip(dataTableAjaxModel.Start).Take(dataTableAjaxModel.Length);//.AsNoTracking();
            PaginatedOrderHeader paginatedOrderHeader = new PaginatedOrderHeader()
            {
                Data = paginatedData,
                TotalCount = AllData.Count()
            };
            return paginatedOrderHeader;
        }
        public int GetCountOfUser(string userId)
        {
            return _context.OrderHeader.Where(s => s.ApplicationUserId.Equals(userId)).Count();
        }
        //Method overloading
        public OrderHeader GetFirstOrDefault(Expression<Func<OrderHeader, bool>>? filter = null)
        {
            OrderHeader query = _context.OrderHeader
                                            .Where(filter)
                                            .Include(a=> a.ApplicationUser)
                                            .Include(o => o.OrderDetails)
                                            .ThenInclude(od => od.Product).FirstOrDefault()!;
            return query;
        }
    }
}
