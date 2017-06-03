using Neutrino.Entities;

namespace Neutrino.Api.Consensus.Responses
{
    public class VoteResponse : IResponse
    {
        public Node Node { get; set; }
        public int CurrentTerm { get; set; }
        public bool VoteGranted { get; set; }

        public VoteResponse(bool voteGranted, int currentTerm, Node node)
        {
            VoteGranted = voteGranted;
            CurrentTerm = currentTerm;
            Node = node;
        }
    }
}