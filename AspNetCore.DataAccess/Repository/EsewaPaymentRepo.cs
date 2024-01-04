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
    public class EsewaPaymentRepo : Repo<EsewaPayment>, IEsewaPaymentRepo
    {
        private readonly AppDbContext _context;
        public EsewaPaymentRepo(AppDbContext context):base(context)
        {
            _context = context;
        }
        public void Update(EsewaPayment payment)
        {
            _context.EsewaPayment.Update(payment);
        }
    }
}
