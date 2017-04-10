using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SceneSkope.PowerBI
{
    public abstract class BasePowerBIClient
    {
        protected static readonly JsonSerializerSettings _schemaSettings = CreateSchemaSettings();
        private static JsonSerializerSettings CreateSchemaSettings()
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

        protected static readonly JsonSerializerSettings _dataSettings = new JsonSerializerSettings
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            Formatting = Formatting.Indented,

        };

        protected HttpClient HttpClient { get; }

        protected BasePowerBIClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        protected async Task<T> AuthenticateSendRequestAndDecodeResultAsync<T>(HttpRequestMessage request, CancellationToken ct)
        {
            using (var response = await AuthenticateAndSendRequestAsync(request, ct).ConfigureAwait(false))
            {
                await CheckForFailureAndLogIfRequired(response).ConfigureAwait(false);
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        protected async Task CheckForFailureAndLogIfRequired(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.Content != null)
                {
                    response.Content.Dispose();
                }
                throw new PowerBIClientException(response.StatusCode, response.ReasonPhrase, result);
            }
        }

        protected async Task<HttpResponseMessage> AuthenticateAndSendRequestAsync(HttpRequestMessage request, CancellationToken ct)
        {
            await AuthenticateRequestAsync(request, ct).ConfigureAwait(false);
            return await HttpClient.SendAsync(request, ct).ConfigureAwait(false);
        }

        protected abstract Task AuthenticateRequestAsync(HttpRequestMessage request, CancellationToken ct);
    }
}
