using Newtonsoft.Json;
using SceneSkope.PowerBI;
using SceneSkope.PowerBI.Authenticators;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RefreshDataset
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var parser = new CommandLineParser.CommandLineParser();
            try
            {
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
                if (!parser.ParsingSucceeded)
                {
                    parser.ShowUsage();
                }
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

                if (!string.IsNullOrWhiteSpace(arguments.GroupId))
                {
                    powerBIClient = powerBIClient.CreateGroupClient(arguments.GroupId);
                }

                if (string.IsNullOrWhiteSpace(arguments.DatasetId))
                {
                    var datasets = await powerBIClient.ListAllDatasetsAsync(ct).ConfigureAwait(false);
                    foreach (var dataset in datasets)
                    {
                        Console.WriteLine($"{dataset.Name} = {dataset.Id}");
                    }
                }
                else
                {
                    Console.WriteLine("About to refresh the dataset");
                    await powerBIClient.RefreshDatasetAsync(arguments.DatasetId, ct).ConfigureAwait(false);
                    Console.WriteLine("Refreshed the dataset");
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