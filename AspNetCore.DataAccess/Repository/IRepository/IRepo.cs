using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.DataAccess.Repository.IRepository
{
    public interface IRepo<T> where T : class
    {
        public IEnumerable<T> GetAll();
        public void Add(T entity);
        public T GetFirstOrDefault(Expression<Func<T,bool>> filter);
        public void Remove(T entity);
        public void RemoveRange(IEnumerable<T> entities);
    }
}
