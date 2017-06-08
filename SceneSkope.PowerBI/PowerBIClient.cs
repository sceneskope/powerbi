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
using System.Collections.Generic;

namespace SceneSkope.PowerBI
{
    public class PowerBIClient : BasePowerBIClient
    {
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
                }
                else
                {
                    if (BaseUrl.StartsWith(BetaBaseUrl))
                    {
                        BaseUrl = BaseUrl.Replace(BetaBaseUrl, DefaultBaseUrl);
                    }
                }
            }
        }

        private readonly IAuthenticator _authenticator;

        public PowerBIClient(HttpClient httpClient, IAuthenticator authenticator) : base(httpClient)
        {
            _authenticator = authenticator;
        }

        public PowerBIClient CreateGroupClient(string id) =>
            new PowerBIClient(HttpClient, _authenticator)
            {
                BaseUrl = $"{BaseUrl}/groups/{id}"
            };

        public Task AddRowsAsync<T>(string datasetId, string tableName, IReadOnlyList<T> rows, CancellationToken ct) =>
            AddRowsAsync(datasetId, tableName, rows, null, ct);

        public async Task AddRowsAsync<T>(string datasetId, string tableName, IReadOnlyList<T> rows, long? sequenceNumber, CancellationToken ct)
        {
            var json = JsonConvert.SerializeObject(new { Rows = rows }, _dataSettings);
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
            var json = JsonConvert.SerializeObject(updatedTable, _schemaSettings);
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
            var json = JsonConvert.SerializeObject(dataset, _schemaSettings);
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

        protected override async Task AuthenticateRequestAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var token = await _authenticator.GetAccessTokenAsync(ct).ConfigureAwait(false);
            ct.ThrowIfCancellationRequested();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
