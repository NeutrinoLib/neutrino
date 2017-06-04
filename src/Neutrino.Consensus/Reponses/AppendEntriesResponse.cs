namespace Neutrino.Consensus.Responses
{
    public class AppendEntriesResponse : IResponse
    {
        public int Term { get; set; }

        public bool WasSuccessful { get; set; }

        public AppendEntriesResponse(int term, bool wasSuccessful)
        {
            Term = term;
            WasSuccessful = wasSuccessful;
        }
    }
}