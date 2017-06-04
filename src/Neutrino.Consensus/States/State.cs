using System;
using Neutrino.Consensus.Events;
using Neutrino.Consensus.Responses;

namespace Neutrino.Consensus.States
{
    public abstract class State : IDisposable
    {
        private bool disposedValue = false;

        public abstract IResponse TriggerEvent(IEvent triggeredEvent);
        
        public virtual void Proceed()
        {
        }

        public virtual void DisposeCore()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposeCore();
                }

                disposedValue = true;
            }
        }

        ~State() {
          Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}