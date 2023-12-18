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
    public class CategoryRepo : Repo<Category>, ICategoryRepo
    {
        private readonly AppDbContext _context;
        public CategoryRepo(AppDbContext context):base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            _context.Category.Update(category);
        }
    }
}
