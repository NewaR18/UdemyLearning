using AspNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.DataAccess.Repository.IRepository
{
    public interface IPaymentKhaltiRepo : IRepo<PaymentKhalti>
    {
        public void Update(PaymentKhalti khaltiPayment);
    }
}
