using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repoistory.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repoistory
{
    public class Repoistory<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;
        public Repoistory(ApplicationDbContext context)
        {
            _context = context;
            dbSet = _context.Set<T>();
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> filter = null, 
            string includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
                query = query.Where(filter);
            if (includeProperties!=null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] {','},
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public T Get(int id)
        {
            return dbSet.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, 
            string includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
                query = query.Where(filter);
            if (includeProperties!=null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, 
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            if (orderby != null)
                return orderby(query).ToList();
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Remove(int id)
        {
            //var entity = dbSet.Find(id);
            //dbSet.Remove(entity);
            
            //2nd way 
            
            var entity=Get (id);
            Remove(entity);
        }

        public void RemoveRamge(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}
