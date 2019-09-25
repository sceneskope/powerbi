using System;
using CommandLineParser.Arguments;

namespace RefreshDataset
{
    internal sealed class Arguments
    {
        [ValueArgument(typeof(string), "client", Optional = false)]
        public string ClientId { get; set; }

        [ValueArgument(typeof(string), "group", Optional = true)]
        public string Group { get; set; }

        public Guid GroupId => Guid.Parse(Group);

        [ValueArgument(typeof(string), "dataset", Optional = true)]
        public string DatasetId { get; set; }

        [ValueArgument(typeof(string), "tokenfile", Optional = false)]
        public string TokenFile { get; set; }
    }
}
