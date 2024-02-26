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
    public class PaymentRefundRepo: Repo<PaymentRefund> , IPaymentRefundRepo
    {
        private readonly AppDbContext _context;
        public PaymentRefundRepo(AppDbContext context):base(context) 
        {
            _context = context;
        }
        public void Update(PaymentRefund paymentRefund)
        {
            _context.PaymentRefund.Update(paymentRefund);
        }
    }
}
