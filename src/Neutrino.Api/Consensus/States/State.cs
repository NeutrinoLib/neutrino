using System;
using Neutrino.Api.Consensus.Events;
using Neutrino.Api.Consensus.Responses;

namespace Neutrino.Api.Consensus.States
{
    public abstract class State : IDisposable
    {
        public virtual void Proceed()
        {
        }

        public virtual IResponse TriggerEvent(IEvent triggeredEvent)
        {
            var leaderRequestEvent = triggeredEvent as LeaderRequestEvent;
            if(leaderRequestEvent != null)
            {
                return new VoteResponse(false);
            }

            return new EmptyResponse();
        }

        public virtual void Dispose()
        {
        }
    }
}