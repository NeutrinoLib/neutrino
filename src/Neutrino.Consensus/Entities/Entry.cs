namespace Neutrino.Consensus.Entities
{
    public class Entry
    {
        public string ObjectType { get; set; }

        public MethodType Method { get; set; }

        public dynamic Value { get; set; }
    }
}