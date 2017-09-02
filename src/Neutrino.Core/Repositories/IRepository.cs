using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Neutrino.Core.Repositories
{
    public interface IRepository<T>
    {
        IEnumerable<T> Get();

        IEnumerable<T> Get(Func<T, bool> predicate);

        T Get(string id);

        void Create(T entity);

        void Update(T entity);

        void Remove(string id);

        void Clear();
    }
}