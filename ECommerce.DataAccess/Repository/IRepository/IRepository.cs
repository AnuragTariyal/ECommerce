using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repoistory.IRepository
{
    public interface IRepository<T> where T:class
    {
        void Add(T entity);     //save
        void Remove(T entity);
        void Remove(int id);
        void RemoveRamge(IEnumerable<T> entity);
        T Get(int id);
        IEnumerable<T> GetAll(
            Expression<Func<T ,bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderby= null,
            string includeProperties=null);                     //category covertype

        T FirstOrDefault(
            Expression<Func<T, bool>> filter = null,                //category covertype
            string includeProperties = null
            );
    }
}
