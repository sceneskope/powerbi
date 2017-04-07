using System;
using System.Collections.Generic;
using System.Text;

namespace SceneSkope.PowerBI
{
    public class ClientConfiguration
    {
        public string ClientId { get; set; }
        public byte[] TokenCacheState { get; set; }
        public string UserName { get; set; }
    }
}
