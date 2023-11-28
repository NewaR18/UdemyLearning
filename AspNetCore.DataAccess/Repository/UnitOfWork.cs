using AspNetCore.DataAccess.Data;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            LibraryRepo=new LibraryRepo(_context);
            CoverTypeRepo = new CoverTypeRepo(_context);
        }
        public ILibraryRepo LibraryRepo { get; private set; }
        public ICoverTypeRepo CoverTypeRepo { get; private set; }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
