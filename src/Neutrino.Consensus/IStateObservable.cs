using Neutrino.Consensus.States;

namespace Neutrino.Consensus
{
    public interface IStateObservable
    {
        void OnStateChanging(State oldState, State newState);
        void OnStateChanged(State oldState, State newState);
    }
}