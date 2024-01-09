using AspNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepo : IRepo<ShoppingCart>
    {
        public void IncrementCount(ShoppingCart cart,int count);
    }
}
