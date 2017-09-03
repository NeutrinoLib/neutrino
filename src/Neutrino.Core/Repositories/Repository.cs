using System;
using System.Collections.Generic;
using System.Linq;
using Neutrino.Core.Infrastructure;
using Neutrino.Entities.Model;

namespace Neutrino.Core.Repositories
{
    public class Repository<T> : IRepository<T> where T: BaseEntity
    {
        private readonly IStoreContext _storeContext;        
        
        public Repository(IStoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public IEnumerable<T> Get()
        {
            var set = _storeContext.Set<T>();
            return set;
        }

        public IEnumerable<T> Get(Func<T, bool> predicate)
        {
            var set = _storeContext.Set<T>();
            var query = set.Where(predicate);
            return query;
        }

        public T Get(string id)
        {
            var set = _storeContext.Set<T>();
            return set.FirstOrDefault(x => x.Id == id);
        }

        public void Create(T entity)
        {
            _storeContext.Add(entity);
        }

        public void Update(T entity)
        {
            _storeContext.Update(entity.Id, entity);
        }

        public void Remove(string id)
        {
            _storeContext.Remove<T>(id);
        }

        public void Clear()
        {
            var set = _storeContext.Set<T>();
            set.Clear();
        }
    }
}