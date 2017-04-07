using Newtonsoft.Json;
using SceneSkope.PowerBI;
using System;
using System.IO;
using System.Net.Http;
using SceneSkope.PowerBI.Authenticators;

namespace PowerBIClientTests
{
    public class TestContext : IDisposable
    {
        public HttpClient HttpClient { get; } = new HttpClient();

        private readonly DeviceCodeAuthenticator _authenticator;

        public ClientConfiguration ClientConfiguration { get; }

        public void Dispose() => HttpClient.Dispose();

        public PowerBIClient CreateClient(IAuthenticator authenticator = null) =>
            new PowerBIClient(HttpClient, authenticator ?? _authenticator);

        private const string TestConfigurationFileName = @"..\..\..\..\PowerBIClientTesting.json";

        public TestContext()
        {
            var configFile = new FileInfo(TestConfigurationFileName);
            if (!configFile.Exists)
            {
                throw new InvalidOperationException($"Unable to find configuration file {configFile.FullName}");
            }

            ClientConfiguration = JsonConvert.DeserializeObject<ClientConfiguration>(File.ReadAllText(TestConfigurationFileName));
            _authenticator = new DeviceCodeAuthenticator(ClientConfiguration.ClientId, ClientConfiguration.TokenCacheState);
        }
    }
}
