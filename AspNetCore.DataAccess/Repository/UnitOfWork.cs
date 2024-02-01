using AspNetCore.DataAccess.Data;
using AspNetCore.DataAccess.Repository.IRepository;
using AspNetCore.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
		private IDbContextTransaction _transaction;
		public UnitOfWork(AppDbContext context)
        {
            _context = context;
            LibraryRepo=new LibraryRepo(_context);
            CoverTypeRepo = new CoverTypeRepo(_context);
            CategoryRepo = new CategoryRepo(_context);
            ProductRepo = new ProductRepo(_context);
            CompanyRepo = new CompanyRepo(_context);
            MenuRepo = new MenuRepo(_context);
            PaymentKhaltiRepo = new PaymentKhaltiRepo(_context);
            EsewaPaymentRepo = new EsewaPaymentRepo(_context);
            ShoppingCartRepo = new ShoppingCartRepo(_context);
            OrderHeaderRepo = new OrderHeaderRepo(_context);
            OrderDetailsRepo = new OrderDetailsRepo(_context);
        }
        public ILibraryRepo LibraryRepo { get; private set; }
        public ICoverTypeRepo CoverTypeRepo { get; private set; }
        public ICategoryRepo CategoryRepo { get; private set; }
        public IProductRepo ProductRepo { get; private set; }  
        public ICompanyRepo CompanyRepo { get; private set; }
        public IMenuRepo MenuRepo { get; private set; }
        public IPaymentKhaltiRepo PaymentKhaltiRepo { get; private set; }
        public IEsewaPaymentRepo EsewaPaymentRepo { get; private set; }
        public IShoppingCartRepo ShoppingCartRepo { get; private set; }
		public IOrderHeaderRepo OrderHeaderRepo { get; private set; }
		public IOrderDetailsRepo OrderDetailsRepo { get; private set; }

		public void BeginTransaction()
		{
			if (_transaction == null)
			{
				_transaction = _context.Database.BeginTransaction();
			}
		}

		public void CommitTransaction()
		{
			_transaction?.Commit();
		}

		public void DisposeTransaction()
		{
			_transaction?.Dispose();
		}

		public void RollbackTransaction()
		{
			_transaction?.Rollback();
		}

		public void Save()
        {
            _context.SaveChanges();
        }
    }
}
