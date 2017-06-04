using Neutrino.Consensus.Entities;

namespace Neutrino.Consensus.Responses
{
    public class RequestVoteResponse : IResponse
    {
        public NodeInfo Node { get; set; }
        public int CurrentTerm { get; set; }
        public bool VoteGranted { get; set; }

        public RequestVoteResponse(bool voteGranted, int currentTerm, NodeInfo node)
        {
            VoteGranted = voteGranted;
            CurrentTerm = currentTerm;
            Node = node;
        }
    }
}