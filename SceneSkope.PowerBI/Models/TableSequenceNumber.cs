using Newtonsoft.Json;

namespace SceneSkope.PowerBI.Models
{
    public class TableSequenceNumber
    {
        [JsonProperty(Required = Required.Always)]
        public string ClientId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public long SequenceNumber { get; set; }

        public override string ToString() => $"{ClientId} {SequenceNumber}";
    }
}