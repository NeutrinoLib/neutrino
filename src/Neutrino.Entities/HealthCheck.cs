using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Neutrino.Entities
{
    public class HealthCheck
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HealthCheckType HealthCheckType { get; set; }

        public string Address { get; set; }

        public int Interval { get; set; }

        public int DeregisterCriticalServiceAfter { get; set; }
    }
}