using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Neutrino.Entities.Model;

namespace Neutrino.Core.Infrastructure
{
    public class StoreContext : IStoreContext
    {
        private readonly ConcurrentDictionary<Type, object> _sets;
        
        private object _lock = new object();

        public StoreContext()
        {
            _sets = new ConcurrentDictionary<Type, object>();
        }

        public IList<T> Set<T>() where T : BaseEntity
        {
            lock(_lock)
            {
                var set = GetOrAddSet<T>();
                return new List<T>(set);
            }
        }

        public void Add<T>(T entity) where T : BaseEntity
        {
            lock(_lock)
            {
                var set = GetOrAddSet<T>();
                set.Add(entity);
            }
        }

        public void Remove<T>(string id) where T : BaseEntity
        {
            lock(_lock)
            {
                var set = GetOrAddSet<T>();
                var entity = set.FirstOrDefault(x => x.Id == id);

                if(entity != null)
                {
                    set.Remove(entity);
                }
            }
        }

        public void Update<T>(string id, T entity) where T : BaseEntity
        {
            lock(_lock)
            {
                var set = GetOrAddSet<T>();
                var entityFromSet = set.FirstOrDefault(x => x.Id == id);

                if(entityFromSet != null)
                {
                    set.Remove(entityFromSet);
                }
                
                set.Add(entity);
            }
        }

        public void Clear<T>() where T : BaseEntity
        {
            lock(_lock)
            {
                var set = GetOrAddSet<T>();
                set.Clear();
            }
        }

        private IList<T> GetOrAddSet<T>()
        {
            var type = typeof(T);
            if (!_sets.TryGetValue(type, out var set))
            {
                set = new List<T>();
                _sets[type] = set;
            }

            return (IList<T>) set;
        }
    }
}