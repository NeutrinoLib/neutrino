namespace Neutrino.Client
{
    public interface INeutrinoClientOptions
    {
        string[] Addresses { get; set; }

        string SecureToken { get; set; }
    }
}