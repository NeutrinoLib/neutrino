using LiteDB;
using Microsoft.Extensions.Options;
using Neutrino.Core.Services.Parameters;
using Neutrino.Entities;

namespace Neutrino.Core.Infrastructure
{
    public class StoreContext : IStoreContext
    {
        public LiteRepository Repository { get; private set; }

        private readonly ApplicationParameters _applicationParameters;

        public StoreContext(IOptions<ApplicationParameters> applicationParameters)
        {
            _applicationParameters = applicationParameters.Value;
            Repository = new LiteRepository(_applicationParameters.ConnectionStrings.DefaultConnection);
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