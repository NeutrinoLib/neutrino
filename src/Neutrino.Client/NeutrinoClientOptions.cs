namespace Neutrino.Client
{
    public class NeutrinoClientOptions : INeutrinoClientOptions
    {
        public string[] Addresses { get; set; }

        public string SecureToken { get; set; }
    }
}