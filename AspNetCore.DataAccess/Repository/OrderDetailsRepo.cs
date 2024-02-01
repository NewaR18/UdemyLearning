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
    public class OrderDetailsRepo : Repo<OrderDetails>, IOrderDetailsRepo
	{
        private readonly AppDbContext _context;
        public OrderDetailsRepo(AppDbContext context):base(context)
        {
            _context = context;
        }

        public void Update(OrderDetails orderDetails)
        {
            _context.OrderDetails.Update(orderDetails);
        }
    }
}
