using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SceneSkope.PowerBI.Models
{
    internal class PowerBIResult<T>
    {
        [JsonProperty(PropertyName = "@odata.context")]
        public string Context { get; set; }

        public T[] Value { get; set; }
    }
}
