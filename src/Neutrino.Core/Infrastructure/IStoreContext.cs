using System;
using Microsoft.Data.Sqlite;
using Neutrino.Entities;

namespace Neutrino.Core.Infrastructure
{
    public interface IStoreContext : IDisposable
    {
        SqliteConnection Repository { get;  }
    }
}