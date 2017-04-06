using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SceneSkope.PowerBI.Models
{
    public class Column
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DataType DataType { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string FormatString { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string SortByColumn { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DataCategory DataCategory { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsHidden { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public AggregationMethod SummarizeBy { get; set; }

        public override string ToString() => $"{Name} {DataType}";
    }
}
