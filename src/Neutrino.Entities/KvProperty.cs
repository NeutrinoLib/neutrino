using Newtonsoft.Json;

namespace Neutrino.Entities
{
    public class KvProperty : BaseEntity
    {
        [JsonIgnore]
        public override string Id 
        {
            get 
            {
                return Key;
            }
            set 
            {
                Key = value;
            }
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}