using AspNetCore.DataAccess.Data;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.DataAccess.Repository
{
    public class ProductRepo : Repo<Product>, IProductRepo
    {
        private readonly AppDbContext _context;
        public ProductRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Product entity)
        {
            Product product = _context.Product.FirstOrDefault(u=>u.Id == entity.Id);
            if (product != null)
            {
                product.Title = entity.Title;
                product.Description = entity.Description;
                product.ISBN =  entity.ISBN;
                product.Author = entity.Author;
                product.ListPrice = entity.ListPrice;
                product.Price = entity.Price;
                product.Price50 = entity.Price50;
                product.Price100 = entity.Price100;
                if(entity.ImageURL != null)
                {
                    product.ImageURL = entity.ImageURL;
                }
            }
        }
    }
}
