using Newtonsoft.Json;
using SceneSkope.PowerBI;
using SceneSkope.PowerBI.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SceneSkope.PowerBI.Authenticators;

namespace PushStreamingSampleRetentionNone
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("usage: PushStreamingSample <configuration file>");
            }
            else
            {
                try
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        Console.CancelKeyPress += (_, ev) =>
                        {
                            ev.Cancel = true;
                            cts.Cancel();
                        };

                        RunAsync(args[0], cts.Token).GetAwaiter().GetResult();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private static async Task RunAsync(string configurationFile, CancellationToken ct)
        {
            using (var httpClient = new HttpClient())
            {
                var configuration = JsonConvert.DeserializeObject<ClientConfiguration>(File.ReadAllText(configurationFile));
                var authenticator = new DeviceCodeAuthenticator(configuration.ClientId, configuration.TokenCacheState);
                var powerBIClient = new PowerBIClient(httpClient, authenticator)
                {
                    UseBeta = true
                };
                var dataset = CreateDatasetDefinition();
                var existingDatasets = await powerBIClient.ListAllDatasetsAsync(ct).ConfigureAwait(false);
                var existingDataset = existingDatasets.SingleOrDefault(d => d.Name == dataset.Name);
                string datasetId;
                if (existingDataset == null)
                {
                    var created = await powerBIClient.CreateDatasetAsync(dataset, DefaultRetentionPolicy.None, ct).ConfigureAwait(false);
                    datasetId = created.Id;
                }
                else
                {
                    datasetId = existingDataset.Id;
                }
                var tableName = dataset.Tables[0].Name;
                var random = new Random();
                while (!ct.IsCancellationRequested)
                {
                    var row = new
                    {
                        TimestampUtc = DateTime.UtcNow,
                        IntCounter = random.Next(0, 1000),
                        FloatCounter = random.NextDouble() * 1000,
                        Flag = random.NextDouble() > 0.5,
                        Label = $"Label {random.Next(0, 1000)}"
                    };
                    await powerBIClient.AddRowsAsync(datasetId, tableName, new[] { row }, ct).ConfigureAwait(false);
                    await Task.Delay(5000, ct).ConfigureAwait(false);
                }
            }
        }

        private static Dataset CreateDatasetDefinition() =>
            new Dataset
            {
                DefaultMode = DatasetMode.PushStreaming,
                Name = "PushStreamingDatasetRetentionNone",
                Tables = new[]
                {
                    new Table
                    {
                        Name = "PushStreamingTableRetentionNone",
                        Columns = new[]
                        {
                            new Column { Name = "TimestampUtc", DataType = DataType.DateTime },
                            new Column { Name = "IntCounter", DataType = DataType.Int64 },
                            new Column { Name = "FloatCounter", DataType = DataType.Double },
                            new Column { Name = "Flag", DataType = DataType.Boolean },
                            new Column { Name = "Label", DataType = DataType.String }
                        }
                    }
                }
            };
    }
}
