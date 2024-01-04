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
    public class PaymentKhaltiRepo : Repo<PaymentKhalti>, IPaymentKhaltiRepo
    {
        private readonly AppDbContext _context;
        public PaymentKhaltiRepo(AppDbContext context):base(context) 
        {
            _context = context;
        }
        public void Update(PaymentKhalti khaltiPayment)
        {
            _context.PaymentKhalti.Update(khaltiPayment);
        }
    }
}
