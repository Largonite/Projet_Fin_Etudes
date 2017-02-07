using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LoginManagement.Dal
{
    public interface IDao<T> : IDisposable where T : class
    {
        List<T> GetAll();
        T Get(int key);
        bool Update(T element);
        bool Delete(T element);
        bool Add(T element);
        bool Exists(T element);
        void SaveChanges();
        T Find(Expression<Func<T, bool>> predicate);
        List<T> FindAll(Expression<Func<T, bool>> predicate);
    }
}
