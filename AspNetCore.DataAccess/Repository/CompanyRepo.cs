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
    public class CompanyRepo : Repo<Company>,ICompanyRepo
    {
        private readonly AppDbContext _context;
        public CompanyRepo(AppDbContext context):base(context)
        {
            _context = context;
        }
        public void Update(Company company)
        {
            _context.Company.Update(company);
        }
    }
}

