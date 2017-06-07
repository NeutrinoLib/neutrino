using Neutrino.Consensus.Events;

namespace Neutrino.Consensus
{
    public interface ILogReplicable
    {
        bool OnLogReplication(AppendEntriesEvent appendEntriesEvent);
    }
}