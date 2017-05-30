using System;
using Neutrino.Api.Consensus.Events;
using Neutrino.Api.Consensus.Responses;

namespace Neutrino.Api.Consensus.States
{
    public abstract class State : IDisposable
    {
        public abstract IResponse TriggerEvent(IEvent triggeredEvent);
        
        public virtual void Proceed()
        {
        }

        public virtual void Dispose()
        {
        }
    }
}