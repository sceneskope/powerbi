using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SceneSkope.PowerBI;

namespace StreamingUISample
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("usage: StreamingUISample <url>");
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

        private static async Task RunAsync(string url, CancellationToken ct)
        {
            using (var httpClient = new HttpClient())
            {
                var powerBIClient = new PowerBIStreamingClient(url, httpClient);
                var random = new Random();
                while (!ct.IsCancellationRequested)
                {
                    var row = new
                    {
                        TimestampUtc = DateTime.UtcNow,
                        IntCounter = random.Next(0, 1000),
                        FloatCounter = random.NextDouble() * 1000
                    };
                    await powerBIClient.AddRowsAsync(new[] { row }, ct).ConfigureAwait(false);
                    await Task.Delay(5000, ct).ConfigureAwait(false);
                }
            }
        }
    }
}