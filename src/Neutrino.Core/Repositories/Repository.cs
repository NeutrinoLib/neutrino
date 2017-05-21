using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Neutrino.Core.Infrastructure;
using Neutrino.Entities;

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
            var query = _storeContext.Repository.Query<T>();
            return query.ToEnumerable();
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> predicate)
        {
            var query = _storeContext.Repository.Query<T>().Where(predicate);
            return query.ToEnumerable();
        }

        public T Get(string id)
        {
            var query = _storeContext.Repository.Query<T>().Where(x => x.Id == id);
            return query.FirstOrDefault();
        }

        public void Create(T entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            if(string.IsNullOrEmpty(entity.Id))
            {
                entity.Id = Guid.NewGuid().ToString();
            }

            _storeContext.Repository.Insert(entity);
        }

        public void Update(string id, T entity)
        {
            entity.Id = id;
            _storeContext.Repository.Update(entity);
        }

        public void Delete(string id)
        {
            _storeContext.Repository.Delete<T>(id);
        }
    }
}