using System;
using CommandLineParser.Arguments;

namespace EmbedInformation
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

        [ValueArgument(typeof(string), "dashboard", Optional = true)]
        public string Dashboard { get; set; }

        public Guid DashboardId => Guid.Parse(Dashboard);

        [ValueArgument(typeof(string), "tile", Optional = true)]
        public string Tile { get; set; }

        public Guid TileId => Guid.Parse(Tile);

        [ValueArgument(typeof(string), "report", Optional = true)]
        public string Report { get; set; }

        public Guid ReportId => Guid.Parse(Report);

        [ValueArgument(typeof(string), "tokenfile", Optional = false)]
        public string TokenFile { get; set; }

        [SwitchArgument("listgroups", false)]
        public bool ListGroups { get; set; }

        [SwitchArgument("listdashboards", false)]
        public bool ListDashboards { get; set; }

        [SwitchArgument("listdatasets", false)]
        public bool ListDatasets { get; set; }

        [SwitchArgument("getparameters", false)]
        public bool GetParameters { get; set; }

        [SwitchArgument("listtiles", false)]
        public bool ListTiles { get; set; }

        [SwitchArgument("listreports", false)]
        public bool ListReports { get; set; }

        [SwitchArgument("embedtoken", false)]
        public bool EmbedToken { get; set; }
    }
}
