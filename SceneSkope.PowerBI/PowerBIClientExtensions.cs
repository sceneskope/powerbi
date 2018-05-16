using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace SceneSkope.PowerBI
{
    public partial class PowerBIClient
    {
        partial void CustomInitialize()
        {
            SerializationSettings.ContractResolver = new DefaultContractResolver();
        }
    }
}
