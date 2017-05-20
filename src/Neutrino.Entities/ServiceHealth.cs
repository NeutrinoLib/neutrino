using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Neutrino.Entities
{
    public class ServiceHealth : BaseEntity
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HealthState HealthState { get; set; }

        public int StatusCode { get; set; }

        public string ResponseMessage { get; set; } 

        [JsonIgnore]
        public string ServiceId { get; set; }
    }
}