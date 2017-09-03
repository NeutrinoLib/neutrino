using System;
using Neutrino.Consensus;
using Neutrino.Consensus.States;
using Neutrino.Core.Services;

namespace Neutrino.Core.Handlers
{
    public class StateObserverHandler : IStateObservable
    {
        private readonly IServicesService _servicesService;

        public StateObserverHandler(IServicesService servicesService)
        {
            _servicesService = servicesService;
        }

        public void OnStateChanged(State oldState, State newState)
        {
            if(newState is Leader)
            {
                _servicesService.RunHealthChecker();
            }
        }

        public void OnStateChanging(State oldState, State newState)
        {
            if(!(newState is Leader))
            {
                _servicesService.StopHealthChecker();
            }
        }
    }
}