using System.Collections.Generic;
using Neutrino.Entities;

namespace Neutrino.Core.Infrastructure
{
    public interface IStoreContext
    {   
        IList<T> Set<T>() where T : BaseEntity;

        void Add<T>(T entity) where T : BaseEntity;

        void Remove<T>(string id) where T : BaseEntity;

        void Update<T>(string id, T entity) where T : BaseEntity;

        void Clear<T>() where T : BaseEntity;
    }
}