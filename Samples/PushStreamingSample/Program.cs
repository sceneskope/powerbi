﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;
using Newtonsoft.Json;
using SceneSkope.PowerBI;
using SceneSkope.PowerBI.Authenticators;
using SceneSkope.PowerBI.Models;

namespace PushStreamingSample
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
                    using var cts = new CancellationTokenSource();
                    Console.CancelKeyPress += (_, ev) =>
{
    ev.Cancel = true;
    cts.Cancel();
};

                    RunAsync(args[0], cts.Token).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
#pragma warning disable ERP023 // Only ex.Message property was observed in exception block!
                    Console.WriteLine($"Error: {ex.Message}");
#pragma warning restore ERP023 // Only ex.Message property was observed in exception block!
                }
            }
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private static async Task RunAsync(string configurationFile, CancellationToken ct)
        {
            var configuration = JsonConvert.DeserializeObject<ClientConfiguration>(File.ReadAllText(configurationFile));
            var tokenProvider = new DeviceCodeTokenProvider(configuration.ClientId, configuration.TokenCacheState);

            using var powerBIClient = new PowerBIClient(new TokenCredentials(tokenProvider));
            var dataset = CreateDatasetDefinition();
            var existingDatasets = await powerBIClient.Datasets.GetDatasetsAsync(ct).ConfigureAwait(false);
            var existingDataset = existingDatasets.Value.SingleOrDefault(d => d.Name == dataset.Name);
            string datasetId;
            if (existingDataset == null)
            {
                var created = await powerBIClient.Datasets.PostDatasetAsync(dataset, DefaultRetentionPolicy.BasicFIFO, ct).ConfigureAwait(false);
                datasetId = ((Dataset)created).Id;
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
                await powerBIClient.Datasets.PostRowsAsync(datasetId, tableName, new[] { row }, ct).ConfigureAwait(false);
                await Task.Delay(5000, ct).ConfigureAwait(false);
            }
        }

        private static CreateDatasetRequest CreateDatasetDefinition() =>
            new CreateDatasetRequest
            {
                DefaultMode = DatasetMode.PushStreaming,
                Name = "PushStreamingDataset",
                Tables = new[]
                {
                    new Table
                    {
                        Name = "PushStreamingTable",
                        Columns = new[]
                        {
                            new Column { Name = "TimestampUtc", DataType = "Datetime" },
                            new Column { Name = "IntCounter", DataType = "Int64" },
                            new Column { Name = "FloatCounter", DataType = "Double" },
                            new Column { Name = "Flag", DataType = "bool" },
                            new Column { Name = "Label", DataType = "string" }
                        }
                    }
                }
            };
    }
}
