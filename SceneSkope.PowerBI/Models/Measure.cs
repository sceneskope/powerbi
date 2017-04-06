using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SceneSkope.PowerBI.Models
{
    public class Measure
    {
        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Expression { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string FormatString { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsHidden { get; set; }

        public override string ToString() => $"{Name} {Expression}";
    }
}
