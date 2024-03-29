﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public ILibraryRepo LibraryRepo { get; }
        public ICoverTypeRepo CoverTypeRepo { get; }
        public ICategoryRepo CategoryRepo {  get; }
        public IProductRepo ProductRepo { get; }
        public ICompanyRepo CompanyRepo { get; }    
        public IMenuRepo MenuRepo { get; }
        public IPaymentKhaltiRepo PaymentKhaltiRepo { get; }
        public IEsewaPaymentRepo EsewaPaymentRepo { get; }
        public IShoppingCartRepo ShoppingCartRepo { get; }
        public IOrderHeaderRepo OrderHeaderRepo { get; }
        public IOrderDetailsRepo OrderDetailsRepo { get; }
        public IPaymentRefundRepo PaymentRefundRepo { get; }
		void Save();
		void BeginTransaction();
		void CommitTransaction();
		void RollbackTransaction();
        void DisposeTransaction();
	}
}
