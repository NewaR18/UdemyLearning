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
        public IEnumerable<T> GetAll(string? IncludeProperties = null)
        {
            IQueryable<T> query = DbSet;
            if (IncludeProperties != null)
            {
                foreach(var property in IncludeProperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries)) 
                {
                    query = query.Include(property);
                }
            }
            return query;
        }
        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? IncludeProperties = null)
        {
            IQueryable<T> query = DbSet.Where(filter);
            if (IncludeProperties != null)
            {
                foreach (var property in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.FirstOrDefault(filter);
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
