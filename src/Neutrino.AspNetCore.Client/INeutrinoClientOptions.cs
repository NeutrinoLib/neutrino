namespace Neutrino.AspNetCore.Client
{
    public interface INeutrinoClientOptions
    {
        string[] Addresses { get; set; }

        string SecureToken { get; set; }
    }
}