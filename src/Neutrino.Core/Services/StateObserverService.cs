using System;
using Neutrino.Consensus;
using Neutrino.Consensus.States;

namespace Neutrino.Core.Services
{
    public class StateObserverService : IStateObservable
    {
        private readonly IServicesService _servicesService;

        public StateObserverService(IServicesService servicesService)
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