namespace Neutrino.Consensus.Entities
{
    public class ConsensusResult
    {
        public bool WasSuccessful { get; protected set; }

        public string Message { get; protected set; }

        protected ConsensusResult()
        {
        }

        public static ConsensusResult CreateSuccessful()
        {
            return new ConsensusResult { WasSuccessful = true };
        }

        public static ConsensusResult CreateError(string message)
        {
            return new ConsensusResult { WasSuccessful = false, Message = message };
        }
    }
}