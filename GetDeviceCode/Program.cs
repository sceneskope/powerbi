using Newtonsoft.Json;
using SceneSkope.PowerBI;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SceneSkope.PowerBI.Authenticators;

namespace GetDeviceCode
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("usage: GetDeviceCode <client id> <output file>");
            }
            else
            {
                try
                {
                    RunAsync(args[0], args[1], CancellationToken.None).GetAwaiter().GetResult();
                    Console.WriteLine("Success");
                }
                catch (Exception ex)
                {
#pragma warning disable ERP023 // Only ex.Message property was observed in exception block!
                    Console.WriteLine($"error: {ex.Message}");
#pragma warning restore ERP023 // Only ex.Message property was observed in exception block!
                }
            }

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private static async Task RunAsync(string clientId, string outputFile, CancellationToken ct)
        {
            var authenticator = new DeviceCodeTokenProvider(clientId,
                (url, deviceCode) => Console.WriteLine($"Go to {url} and enter device code {deviceCode}"));

            await authenticator.GetAccessTokenAsync(ct).ConfigureAwait(false);
            var state = authenticator.GetSerializedState();

            var clientConfiguration = new ClientConfiguration { ClientId = clientId, TokenCacheState = state };
            File.WriteAllText(outputFile, JsonConvert.SerializeObject(clientConfiguration));
        }
    }
}
