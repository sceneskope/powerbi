using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SceneSkope.PowerBI.Models
{
    public class Relationship
    {
        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public CrossFilteringBehaviour CrossFilteringBehaviour { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string FromTable { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string FromColumn { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ToTable { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ToColumn { get; set; }

        public override string ToString() => Name;
    }
}
