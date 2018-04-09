using CommandLineParser.Arguments;

namespace EmbedInformation
{
    internal sealed class Arguments
    {
        [ValueArgument(typeof(string), "client", Optional = false)]
        public string ClientId { get; set; }

        [ValueArgument(typeof(string), "group", Optional = true)]
        public string GroupId { get; set; }

        [ValueArgument(typeof(string), "dataset", Optional = true)]
        public string DatasetId { get; set; }

        [ValueArgument(typeof(string), "dashboard", Optional = true)]
        public string DashboardId { get; set; }

        [ValueArgument(typeof(string), "tile", Optional = true)]
        public string TileId { get; set; }

        [ValueArgument(typeof(string), "report", Optional = true)]
        public string ReportId { get; set; }

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
