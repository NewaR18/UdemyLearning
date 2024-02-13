using AspNetCore.Models;
using AspNetCore.Models.JSModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepo : IRepo<OrderHeader>
    {
        public void Update(OrderHeader orderHeader);
        public void UpdateStatus(int orderId, string orderStatus, string? paymentStatus = null);
        public void UpdateStripeData(int orderId, string sessionId, string paymentIntentId);
        public new IEnumerable<OrderHeader> GetAll(Expression<Func<OrderHeader, bool>>? filter = null, string? IncludeProperties = null);
        public PaginatedOrderHeader GetPaginatedRows(int skip, int length, List<ColumnFilterModel> Columns);

    }
}
