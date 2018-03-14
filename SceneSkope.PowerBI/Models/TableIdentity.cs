using Newtonsoft.Json;

namespace SceneSkope.PowerBI.Models
{
    public class TableIdentity
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        public override string ToString() => Name;
    }
}