using System;
using LiteDB;
using Neutrino.Entities;

namespace Neutrino.Core.Infrastructure
{
    public interface IStoreContext : IDisposable
    {
        LiteRepository Repository { get;  }
    }
}