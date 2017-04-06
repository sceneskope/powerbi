using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SceneSkope.PowerBI.Models;

namespace SceneSkope.PowerBI
{
    public class PowerBIClient
    {
        public string AuthorityUri { get; set; } = "https://login.windows.net/common/oauth2/authorize";

        public string RedirectUri { get; set; } = "https://login.live.com/oauth20_desktop.srf";

        public string ResourceUri { get; set; } = "https://analysis.windows.net/powerbi/api";

        private const string DefaultBaseUrl = "https://api.powerbi.com/v1.0/myorg";
        private const string BetaBaseUrl = "https://api.powerbi.com/beta/myorg";

        public string BaseUrl { get; set; } = DefaultBaseUrl;

        public bool UseBeta
        {
            get => BaseUrl.StartsWith(BetaBaseUrl);
            set
            {
                if (value)
                {
                    if (BaseUrl.StartsWith(DefaultBaseUrl))
                    {
                        BaseUrl = BaseUrl.Replace(DefaultBaseUrl, BetaBaseUrl);
                    }
                } else
                {
                    if (BaseUrl.StartsWith(BetaBaseUrl))
                    {
                        BaseUrl = BaseUrl.Replace(BetaBaseUrl, DefaultBaseUrl);
                    }
                }
            }
        }

        private static readonly JsonSerializerSettings _settings = CreateSettings();
        private static JsonSerializerSettings CreateSettings()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                Formatting = Formatting.None,

            };
            settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            return settings;
        }

        public string ClientId { get; }
        private readonly HttpClient _httpClient;
        private readonly AuthenticationContext _authenticationContext;
        private string _accessToken;
        private readonly Action<string, string> _notifyDeviceCodeRequest;

        public PowerBIClient(string clientId, HttpClient httpClient, byte[] tokenState)
        {
            if (tokenState == null)
            {
                throw new ArgumentNullException($"{nameof(tokenState)} should not be null");
            }

            ClientId = clientId;
            _httpClient = httpClient;
            _authenticationContext = new AuthenticationContext(AuthorityUri, new TokenCache(tokenState));
        }

        public PowerBIClient(string clientId, HttpClient httpClient, Action<string, string> notifyDeviceCodeRequest)
        {
            ClientId = clientId;
            _httpClient = httpClient;
            _authenticationContext = new AuthenticationContext(AuthorityUri, new TokenCache());
            _notifyDeviceCodeRequest = notifyDeviceCodeRequest;
        }

        private PowerBIClient(string clientId, HttpClient httpClient, AuthenticationContext authenticationContext)
        {
            ClientId = clientId;
            _httpClient = httpClient;
            _authenticationContext = authenticationContext;
        }

        public PowerBIClient CreateGroupClient(string id)=>
            new PowerBIClient(ClientId, _httpClient, _authenticationContext)
            {
                BaseUrl = $"{DefaultBaseUrl}/groups/{id}"
            };

        public byte[] GetSerializedState()
        {
            var state = _authenticationContext.TokenCache.Serialize();
            _authenticationContext.TokenCache.HasStateChanged = false;
            return state;
        }

        public bool HasStateChanged => _authenticationContext.TokenCache.HasStateChanged;

#pragma warning disable RCS1163 // Unused parameter.
        public async Task<string> GetAccessTokenAsync(CancellationToken ct)
#pragma warning restore RCS1163 // Unused parameter.
        {
            if (_accessToken == null)
            {
                if (_authenticationContext.TokenCache.Count > 0)
                {
                    var result = await _authenticationContext.AcquireTokenSilentAsync(ResourceUri, ClientId).ConfigureAwait(false);
                    _accessToken = result.AccessToken;
                    return _accessToken;
                }
            }

            if (_accessToken == null)
            {
                var codeResult = await _authenticationContext.AcquireDeviceCodeAsync(ResourceUri, ClientId).ConfigureAwait(false);
                _notifyDeviceCodeRequest(codeResult.VerificationUrl, codeResult.UserCode);
                var result = await _authenticationContext.AcquireTokenByDeviceCodeAsync(codeResult).ConfigureAwait(false);
                _accessToken = result.AccessToken;
            }
            else
            {
                var result = await _authenticationContext.AcquireTokenSilentAsync(ResourceUri, ClientId).ConfigureAwait(false);
                _accessToken = result.AccessToken;
            }

            return _accessToken;
        }

        public Task AddRowsAsync(string datasetId, string tableName, object[] rows, CancellationToken ct) =>
            AddRowsAsync(datasetId, tableName, rows, null, ct);

        public async Task AddRowsAsync(string datasetId, string tableName, object[] rows, long? sequenceNumber, CancellationToken ct)
        {
            var json = JsonConvert.SerializeObject(new { Rows = rows }, _settings);
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/datasets/{datasetId}/tables/{tableName}/rows")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            })
            {
                if (sequenceNumber.HasValue)
                {
                    request.Headers.Add("X-PowerBI-PushData-SequenceNumber", sequenceNumber.Value.ToString());
                }
                using (var response = await AuthenticateAndSendRequestAsync(request, ct).ConfigureAwait(false))
                {
                    await CheckForFailureAndLogIfRequired(response).ConfigureAwait(false);
                }
            }
        }

        public async Task ClearRowsAsync(string datasetId, string tableName, CancellationToken ct)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"{BaseUrl}/datasets/{datasetId}/tables/{tableName}/rows"))
            using (var response = await AuthenticateAndSendRequestAsync(request, ct).ConfigureAwait(false))
            {
                await CheckForFailureAndLogIfRequired(response).ConfigureAwait(false);
            }
        }

        public async Task<TableSequenceNumber[]> GetTableSequenceNumbersAsync(string datasetId, string tableName, CancellationToken ct)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/datasets/{datasetId}/tables/{tableName}/sequenceNumbers"))
            {
                var response = await AuthenticateSendRequestAndDecodeResultAsync<PowerBIResult<TableSequenceNumber>>(request, ct).ConfigureAwait(false);
                return response.Value;
            }
        }

        public async Task UpdateDatasetTableAsync(string id, Table updatedTable, CancellationToken ct)
        {
            var json = JsonConvert.SerializeObject(updatedTable, _settings);
            using (var request = new HttpRequestMessage(HttpMethod.Put, $"{BaseUrl}/datasets/{id}/tables/{updatedTable.Name}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            })
            using (var response = await AuthenticateAndSendRequestAsync(request, ct).ConfigureAwait(false))
            {
                await CheckForFailureAndLogIfRequired(response).ConfigureAwait(false);
            }
        }

        public async Task<PowerBIIdentity[]> ListAllDatasetsAsync(CancellationToken ct)
        {
            Console.WriteLine($"Using BaseURL {BaseUrl}");
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/datasets"))
            {
                var response = await AuthenticateSendRequestAndDecodeResultAsync<PowerBIResult<PowerBIIdentity>>(request, ct).ConfigureAwait(false);
                return response.Value;
            }
        }

        public async Task GetDatasetAsync(string id, CancellationToken ct)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/datasets/{id}"))
            using (var response = await AuthenticateAndSendRequestAsync(request, ct).ConfigureAwait(false))
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        public async Task GetTableAsync(string id, string table, CancellationToken ct)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/datasets/{id}/tables/{table}"))
            using (var response = await AuthenticateAndSendRequestAsync(request, ct).ConfigureAwait(false))
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        public async Task<TableIdentity[]> ListAllTablesAsync(string id, CancellationToken ct)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/datasets/{id}/tables"))
            {
                var response = await AuthenticateSendRequestAndDecodeResultAsync<PowerBIResult<TableIdentity>>(request, ct).ConfigureAwait(false);
                return response.Value;
            }
        }

        public async Task<PowerBIIdentity> CreateDatasetAsync(Dataset dataset, DefaultRetentionPolicy defaultRetentionPolicy, CancellationToken ct)
        {
            var json = JsonConvert.SerializeObject(dataset, _settings);
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/datasets?defaultRetentionPolicy={defaultRetentionPolicy}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            })
            {
                var identity = await AuthenticateSendRequestAndDecodeResultAsync<PowerBIIdentity>(request, ct).ConfigureAwait(false);
                return identity;
            }
        }

        public async Task DeleteDatasetAsync(string id, CancellationToken ct)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"{BaseUrl}/datasets/{id}"))
            using (var response = await AuthenticateAndSendRequestAsync(request, ct).ConfigureAwait(false))
            {
                await CheckForFailureAndLogIfRequired(response).ConfigureAwait(false);
            }
        }

        public async Task<PowerBIIdentity[]> ListAllGroupsAsync(CancellationToken ct)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/groups"))
            {
                var response = await AuthenticateSendRequestAndDecodeResultAsync<PowerBIResult<PowerBIIdentity>>(request, ct).ConfigureAwait(false);
                return response.Value;
            }
        }

        private async Task<T> AuthenticateSendRequestAndDecodeResultAsync<T>(HttpRequestMessage request, CancellationToken ct)
        {
            using (var response = await AuthenticateAndSendRequestAsync(request, ct).ConfigureAwait(false))
            {
                await CheckForFailureAndLogIfRequired(response).ConfigureAwait(false);
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var decoded = JsonConvert.DeserializeObject<T>(json);
                return decoded;
            }
        }

        private async Task CheckForFailureAndLogIfRequired(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
        }

        private async Task<HttpResponseMessage> AuthenticateAndSendRequestAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var token = await GetAccessTokenAsync(ct).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.SendAsync(request, ct).ConfigureAwait(false);
        }
    }
}
