namespace Neutrino.Api.Consensus.Responses
{
    public class VoteResponse : IResponse
    {
        public bool VoteValue { get; set; }

        public VoteResponse(bool voteValue)
        {
            VoteValue = voteValue;
        }
    }
}