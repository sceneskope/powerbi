using Newtonsoft.Json;
using SceneSkope.PowerBI;
using System;
using System.IO;
using System.Net.Http;

namespace PowerBITests
{
    public class TestContext : IDisposable
    {
        public HttpClient HttpClient { get; } = new HttpClient();

        public void Dispose() => HttpClient.Dispose();

        public PowerBIClient CreateClient() =>
            new PowerBIClient(_clientConfiguration.ClientId, HttpClient, _clientConfiguration.TokenCacheState);

        private const string TestConfigurationFileName = @"..\..\..\..\PowerBIClientTesting.json";

        private readonly ClientConfiguration _clientConfiguration;

        public TestContext()
        {
            var configFile = new FileInfo(TestConfigurationFileName);
            if (!configFile.Exists)
            {
                throw new InvalidOperationException($"Unable to find configuration file {configFile.FullName}");
            }

            _clientConfiguration = JsonConvert.DeserializeObject<ClientConfiguration>(File.ReadAllText(TestConfigurationFileName));
        }
    }
}
