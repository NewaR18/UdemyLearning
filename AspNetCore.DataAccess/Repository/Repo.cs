using AspNetCore.DataAccess.Data;
using AspNetCore.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.DataAccess.Repository
{
    public class Repo<T> : IRepo<T> where T : class
    {
        private readonly AppDbContext _context;
        internal DbSet<T> DbSet;
        public Repo(AppDbContext context) { 
            _context = context;
            this.DbSet = _context.Set<T>();
        }
        public void Add(T entity)
        {
            DbSet.Add(entity);
        }
        public IEnumerable<T> GetAll()
        {
            return DbSet;
        }
        public T GetFirstOrDefault(Expression<Func<T, bool>> filter)
        {
            return DbSet.Where(filter).FirstOrDefault();
        }
        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }
    }
}
