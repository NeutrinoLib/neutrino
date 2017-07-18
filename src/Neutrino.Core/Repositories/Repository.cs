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
            using(var transaction = _storeContext.Repository.BeginTrans())
            {
                entity.CreatedDate = DateTime.UtcNow;
                if(string.IsNullOrEmpty(entity.Id))
                {
                    entity.Id = Guid.NewGuid().ToString();
                }

                _storeContext.Repository.Insert(entity);
                transaction.Commit();
            }
        }

        public void Update(T entity)
        {
            using(var transaction = _storeContext.Repository.BeginTrans())
            {
                _storeContext.Repository.Update(entity);
                transaction.Commit();
            }
        }

        public void Delete(string id)
        {
            using(var transaction = _storeContext.Repository.BeginTrans())
            {
                _storeContext.Repository.Delete<T>(id);
                transaction.Commit();
            }
        }

        public void Clear()
        {
            var collectionName = typeof(T).Name;

            if(_storeContext.Repository.Database.CollectionExists(collectionName))
            {
                _storeContext.Repository.Database.DropCollection(collectionName);
            }
        }
    }
}