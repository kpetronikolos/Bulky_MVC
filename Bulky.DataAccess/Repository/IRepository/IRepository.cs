using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
     internal interface IRepository<T> where T : class
     {
          IEnumerable<T> GetAll();
          T Get(int id);
          void Add(T entity);
          void Remove(T entity);
          void RemoveRange(IEnumerable<T> entities);
     }
}
