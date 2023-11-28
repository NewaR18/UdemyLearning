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
    public class CoverTypeRepo : Repo<CoverType>, ICoverTypeRepo
    {
        private readonly AppDbContext _context;
        public CoverTypeRepo(AppDbContext context) :base(context) 
        {
            _context = context;
        }
        public void Update(CoverType coverType)
        {
            _context.CoverType.Update(coverType);
        }
    }
}
