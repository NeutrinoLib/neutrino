namespace Neutrino.Client.Specs
{
    public static class NeutrinoClientFactory
    {
        public static NeutrinoClient GetNeutrinoClient()
        {
            var serverAddress = "http://localhost:5000";

            var neutrinoClientOptions = new NeutrinoClientOptions();
            neutrinoClientOptions.Addresses = new string[] { serverAddress };
			neutrinoClientOptions.SecureToken = "4e57e961-5f2e-4b24-893f-7842c5ccff97";

			var httpRequestService = new HttpRequestService(neutrinoClientOptions);
			return new NeutrinoClient(httpRequestService);
        }
    }
}