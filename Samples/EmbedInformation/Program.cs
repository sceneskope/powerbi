using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Newtonsoft.Json;
using SceneSkope.PowerBI;
using SceneSkope.PowerBI.Authenticators;
using SceneSkope.PowerBI.Models;

namespace EmbedInformation
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var parser = new CommandLineParser.CommandLineParser();
                var arguments = new Arguments();
                parser.ExtractArgumentAttributes(arguments);
                parser.ParseCommandLine(args);
                if (!parser.ParsingSucceeded)
                {
                    parser.ShowUsage();
                }
                else
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        Console.CancelKeyPress += (_, a) =>
                        {
                            a.Cancel = true;
                            cts.Cancel();
                        };

                        ServiceClientTracing.IsEnabled = true;
                        ServiceClientTracing.AddTracingInterceptor(new ConsoleTracingInterceptor());

                        RunAsync(arguments, cts.Token).GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception ex)
            {
#pragma warning disable ERP023 // Only ex.Message property was observed in exception block!
                Console.WriteLine($"error: {ex.Message}");
#pragma warning restore ERP023 // Only ex.Message property was observed in exception block!
            }

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private static async Task RunAsync(Arguments arguments, CancellationToken ct)
        {
            ITokenProvider tokenProvider;
            if (!string.IsNullOrWhiteSpace(arguments.TokenFile) && File.Exists(arguments.TokenFile))
            {
                var configuration = JsonConvert.DeserializeObject<ClientConfiguration>(File.ReadAllText(arguments.TokenFile));
                tokenProvider = new DeviceCodeTokenProvider(configuration.ClientId, configuration.TokenCacheState);
            }
            else
            {
                tokenProvider = new DeviceCodeTokenProvider(arguments.ClientId,
                                (url, deviceCode) => Console.WriteLine($"Go to {url} and enter device code {deviceCode}"));
            }

            var powerBIClient = new PowerBIClient(new TokenCredentials(tokenProvider));

            if (arguments.ListGroups)
            {
                var groups = await powerBIClient.Groups.GetGroupsAsync(ct).ConfigureAwait(false);
                Console.WriteLine($"Got {groups.Value.Count} groups");
                foreach (var group in groups.Value)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(group));
                }
            }

            var noGroup = string.IsNullOrWhiteSpace(arguments.GroupId);

            if (arguments.ListDatasets)
            {
                var datasets = await (noGroup ? powerBIClient.Datasets.GetDatasetsAsync(ct) : powerBIClient.Datasets.GetDatasetsInGroupAsync(arguments.GroupId, ct)).ConfigureAwait(false);
                Console.WriteLine($"Got {datasets.Value.Count} datasets");
                foreach (var dataset in datasets.Value)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(dataset));
                }
            }

            if (arguments.ListDashboards)
            {
                var dashboards = await (noGroup ? powerBIClient.Dashboards.GetDashboardsAsync(ct) : powerBIClient.Dashboards.GetDashboardsInGroupAsync(arguments.GroupId, ct)).ConfigureAwait(false);
                Console.WriteLine($"Got {dashboards.Value.Count} dashboards");
                foreach (var dashboard in dashboards.Value)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(dashboard));
                }
            }

            if (arguments.ListReports)
            {
                var reports = await (noGroup ? powerBIClient.Reports.GetReportsAsync(ct) : powerBIClient.Reports.GetReportsInGroupAsync(arguments.GroupId, ct)).ConfigureAwait(false);
                Console.WriteLine($"Got {reports.Value.Count} reports");
                foreach (var dashboard in reports.Value)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(dashboard));
                }
            }

            if (arguments.ListTiles)
            {
                var tiles = await (noGroup ? powerBIClient.Dashboards.GetTilesAsync(arguments.DashboardId, ct) : powerBIClient.Dashboards.GetTilesInGroupAsync(arguments.GroupId, arguments.DashboardId, ct)).ConfigureAwait(false);
                Console.WriteLine($"Got {tiles.Value.Count} tiles");
                foreach (var tile in tiles.Value)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(tile));
                }
            }

            if (arguments.EmbedToken && !string.IsNullOrWhiteSpace(arguments.ReportId))
            {
                var report = await (noGroup ? powerBIClient.Reports.GetReportAsync(arguments.ReportId, ct) : powerBIClient.Reports.GetReportInGroupAsync(arguments.GroupId, arguments.ReportId, ct)).ConfigureAwait(false);
                Console.WriteLine($"Report: {JsonConvert.SerializeObject(report)}");
                var request = new GenerateTokenRequest { AccessLevel = "View" };
                var token = await (noGroup ? powerBIClient.Reports.GenerateTokenAsync(arguments.ReportId, request, ct) : powerBIClient.Reports.GenerateTokenInGroupAsync(arguments.GroupId, arguments.ReportId, request, ct)).ConfigureAwait(false);
                Console.WriteLine($"Report token: {JsonConvert.SerializeObject(token)}");
            }

            if (arguments.EmbedToken && !string.IsNullOrWhiteSpace(arguments.TileId) && !string.IsNullOrWhiteSpace(arguments.DashboardId))
            {
                var tile = await (noGroup ? powerBIClient.Dashboards.GetTileAsync(arguments.DashboardId, arguments.TileId, ct) : powerBIClient.Dashboards.GetTileInGroupAsync(arguments.GroupId, arguments.DashboardId, arguments.TileId, ct)).ConfigureAwait(false);
                Console.WriteLine($"Tile: {JsonConvert.SerializeObject(tile)}");
                var request = new GenerateTokenRequest { AccessLevel = "View" };
                var token = await (noGroup ? powerBIClient.Dashboards.GenerateTokenAsync(arguments.DashboardId, request, ct) : powerBIClient.Dashboards.GenerateTokenInGroupAsync(arguments.GroupId, arguments.DashboardId, request, ct)).ConfigureAwait(false);
                Console.WriteLine($"Dashboard Token: {JsonConvert.SerializeObject(token)}");
                token = await (noGroup ? powerBIClient.Tiles.GenerateTokenAsync(arguments.DashboardId, arguments.TileId, request, ct) : powerBIClient.Tiles.GenerateTokenInGroupAsync(arguments.GroupId, arguments.DashboardId, arguments.TileId, request, ct)).ConfigureAwait(false);
                Console.WriteLine($"Tile Token: {JsonConvert.SerializeObject(token)}");
            }

            if (arguments.GetParameters && !string.IsNullOrWhiteSpace(arguments.DatasetId)) {
                var parameters = await (noGroup ? powerBIClient.Datasets.GetParametersAsync(arguments.DatasetId, ct) : powerBIClient.Datasets.GetParametersInGroupAsync(arguments.GroupId, arguments.DatasetId, ct)).ConfigureAwait(false);
                Console.WriteLine($"Parameters: {JsonConvert.SerializeObject(parameters)}");
            }

            if (!string.IsNullOrWhiteSpace(arguments.TokenFile) && tokenProvider is DeviceCodeTokenProvider deviceCodeProvider)
            {
                var state = deviceCodeProvider.GetSerializedState();
                var clientConfiguration = new ClientConfiguration { ClientId = arguments.ClientId, TokenCacheState = state };
                File.WriteAllText(arguments.TokenFile, JsonConvert.SerializeObject(clientConfiguration));
            }
        }
    }
}
