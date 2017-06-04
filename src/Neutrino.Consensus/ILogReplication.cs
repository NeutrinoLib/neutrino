using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Consensus.Entities;

namespace Neutrino.Consensus
{
    public interface ILogReplication
    {
        Task<ConsensusResult> DistributeEntry(object objectData, MethodType method);
    }
}