using LiteDB;
using Neutrino.Entities;

namespace Neutrino.Core.Infrastructure
{
    public class StoreContext : IStoreContext
    {
        public LiteRepository Repository { get; private set; }

        public StoreContext()
        {
            Repository = new LiteRepository("nosqltore.db");
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Repository.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}