using Neutrino.Entities;

namespace Neutrino.Api.Consensus
{
    public interface ILeaderElectionService
    {
        void Run();
        
        void ReceiveHearbeat(Node node);

        bool ReceiveLeaderRequest(Node node);
    }
}