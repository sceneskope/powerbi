using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SceneSkope.PowerBI.Models
{
    [JsonConverter(typeof(StringEnumConverter), false)]
    public enum DatasetMode
    {
        Push,
        Streaming,
        PushStreaming
    }
}
