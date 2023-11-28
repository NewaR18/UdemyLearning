using System;
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
        void Save();
    }
}
