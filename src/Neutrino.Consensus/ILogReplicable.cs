using Neutrino.Consensus.Events;

namespace Neutrino.Consensus
{
    /// <summary>
    /// Interface for log replication events.
    /// </summary>
    public interface ILogReplicable
    {
        /// <summary>
        /// Method which will be executed when raft algorithm receive append entries event.
        /// </summary>
        /// <param name="appendEntriesEvent">Appen entried event data.</param>
        /// <returns>True if event was handled correctly.</returns>
        bool OnLogReplication(AppendEntriesEvent appendEntriesEvent);

        /// <summary>
        /// Methot which will be executed when all logs shoud be cleared. This will happened when
        /// new service instance is started and leader is sending first message.
        /// </summary>
        /// <returns>True if event was handled correctly.</returns>
        bool OnClearLog();
        
        /// <summary>
        /// Method which will be executed when other service wants to retrieve 
        /// </summary>
        /// <returns></returns>
        AppendEntriesEvent OnGetFullLog();
    }
}