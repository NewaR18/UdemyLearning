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
    public class ShoppingCartRepo : Repo<ShoppingCart>, IShoppingCartRepo
    {
        private readonly AppDbContext _context;
        public ShoppingCartRepo(AppDbContext context):base(context)
        {
            _context = context;
        }
        public void IncrementCount(ShoppingCart cart, int count)
        {
            cart.Count += count;
        }
    }
}
