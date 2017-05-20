namespace Neutrino.Entities
{
    public class Service : BaseEntity
    {
        public string ServiceType { get; set; }

        public string Address { get; set; }

        public string[] Tags { get; set; }

        public HealthCheck HealthCheck { get; set; }
    }
}