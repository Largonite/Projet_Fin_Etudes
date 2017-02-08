using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LoginManagement.Dal
{
    public class GenericDao<T> : IDao<T> where T : class, IEntity
    {
        private PaimenEntities _context;

        public GenericDao(PaimenEntities context)
        {
            this._context = context;
        }

        public bool Add(T element)
        {
            if(element.GetId() == 0)
            {
                this._context.Set<T>().Add(element);
                return true;
            }

            if(!this.Exists(element))
            {
                this._context.Set<T>().Add(element);
                return true;
            }
            return false;
        }

        public bool Delete(T element)
        {
            if (this.Exists(element))
            {
                this._context.Set<T>().Remove(element);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            this._context.Dispose();
        }

        public bool Exists(T element)
        {
            try
            {
                T exists = this._context.Set<T>().Find(element.GetId());
                return exists != null;
            }
            catch(DataException e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
            
        }

        public T Find(Expression<Func<T,bool>> expression)
        {
            return this._context.Set<T>().Where(expression).FirstOrDefault();
        }

        public List<T> FindAll(Expression<Func<T, bool>> predicate)
        {
            return this._context.Set<T>().Where(predicate).ToList();
        }

        public T Get(int key)
        {
             return this._context.Set<T>().Find(key);
        }

        public List<T> GetAll()
        {
            return this._context.Set<T>().ToList<T>();
        }

        public void SaveChanges()
        {
            try
            {
                this._context.SaveChanges();
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        public bool Update(T element)
        {
            T previous = this.Get(element.GetId());
            if(previous == null)
                return false;

            foreach(var property in element.GetType().GetProperties())
            {
                property.SetValue(previous, property.GetValue(element, null));
            }
            return true;
        }


    }
}