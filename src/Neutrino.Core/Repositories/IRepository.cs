using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Neutrino.Core.Repositories
{
    public interface IRepository<T>
    {
        IQueryable<T> Get();

        IQueryable<T> Get(Func<T, bool> predicate);

        T Get(string id);

        void Create(T entity);

        void Update(T entity);

        void Remove(string id);

        void Clear();
    }
}