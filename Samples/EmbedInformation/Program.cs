using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SceneSkope.PowerBI;
using SceneSkope.PowerBI.Authenticators;

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

                        RunAsync(arguments, cts.Token).GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error: {ex.Message}");
            }

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private static async Task RunAsync(Arguments arguments, CancellationToken ct)
        {
            using (var httpClient = new HttpClient())
            {
                DeviceCodeAuthenticator authenticator;
                if (!string.IsNullOrWhiteSpace(arguments.TokenFile) && File.Exists(arguments.TokenFile))
                {
                    var configuration = JsonConvert.DeserializeObject<ClientConfiguration>(File.ReadAllText(arguments.TokenFile));
                    authenticator = new DeviceCodeAuthenticator(configuration.ClientId, configuration.TokenCacheState);
                }
                else
                {
                    authenticator = new DeviceCodeAuthenticator(arguments.ClientId,
                                    (url, deviceCode) => Console.WriteLine($"Go to {url} and enter device code {deviceCode}"));
                }

                var powerBIClient = new PowerBIClient(httpClient, authenticator)
                {
                    UseBeta = true
                };

                if (arguments.ListGroups)
                {
                    var groups = await powerBIClient.ListAllGroupsAsync(ct).ConfigureAwait(false);
                    Console.WriteLine($"Got {groups.Length} groups");
                    foreach (var group in groups)
                    {
                        Console.WriteLine($"{JsonConvert.SerializeObject(group)}");
                    }
                }

                if (!string.IsNullOrWhiteSpace(arguments.GroupId))
                {
                    powerBIClient = powerBIClient.CreateGroupClient(arguments.GroupId);
                }

                if (arguments.ListDashboards)
                {
                    var dashboards = await powerBIClient.ListAllDashboardsAsync(ct).ConfigureAwait(false);
                    Console.WriteLine($"Got {dashboards.Length} dashboards");
                    foreach (var dashboard in dashboards)
                    {
                        Console.WriteLine($"{JsonConvert.SerializeObject(dashboard)}");
                    }
                }

                if (arguments.ListReports)
                {
                    var reports = await powerBIClient.ListAllReportsAsync(ct).ConfigureAwait(false);
                    Console.WriteLine($"Got {reports.Length} reports");
                    foreach (var dashboard in reports)
                    {
                        Console.WriteLine($"{JsonConvert.SerializeObject(dashboard)}");
                    }
                }

                if (arguments.ListTiles)
                {
                    var tiles = await powerBIClient.ListAllTilesAsync(arguments.DashboardId, ct).ConfigureAwait(false);
                    Console.WriteLine($"Got {tiles.Length} tiles");
                    foreach (var tile in tiles)
                    {
                        Console.WriteLine($"{JsonConvert.SerializeObject(tile)}");
                    }
                }

                if (arguments.EmbedToken && !string.IsNullOrWhiteSpace(arguments.ReportId))
                {
                    var report = await powerBIClient.GetReportAsync(arguments.ReportId, ct).ConfigureAwait(false);
                    Console.WriteLine($"Report: {JsonConvert.SerializeObject(report)}");
                    var token = await powerBIClient.GetReportTokenAsync(arguments.ReportId, "View", ct).ConfigureAwait(false);
                    Console.WriteLine($"Report token: {JsonConvert.SerializeObject(token)}");
                }

                if (arguments.EmbedToken && !string.IsNullOrWhiteSpace(arguments.TileId) && !string.IsNullOrWhiteSpace(arguments.DashboardId))
                {
                    var tile = await powerBIClient.GetTileAsync(arguments.DashboardId, arguments.TileId, ct).ConfigureAwait(false);
                    Console.WriteLine($"Tile: {JsonConvert.SerializeObject(tile)}");
                    var token = await powerBIClient.GetDashboardTokenAsync(arguments.DashboardId, "View", ct).ConfigureAwait(false);
                    Console.WriteLine($"Dashboard Token: {JsonConvert.SerializeObject(token)}");
                    token = await powerBIClient.GetTileTokenAsync(arguments.DashboardId, arguments.TileId, "View", ct).ConfigureAwait(false);
                    Console.WriteLine($"Tile Token: {JsonConvert.SerializeObject(token)}");
                }

                if (!string.IsNullOrWhiteSpace(arguments.TokenFile))
                {
                    var state = authenticator.GetSerializedState();
                    var clientConfiguration = new ClientConfiguration { ClientId = arguments.ClientId, TokenCacheState = state };
                    File.WriteAllText(arguments.TokenFile, JsonConvert.SerializeObject(clientConfiguration));
                }
            }
        }
    }
}