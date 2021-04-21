﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using SceneSkope.PowerBI.Models;

namespace SceneSkope.PowerBI
{
    /// <summary>
    /// Client wrapper for Power BI Imports REST Api
    /// </summary>
    public partial class Imports : IServiceOperations<PowerBIClient>, IImports
    {
        private const int MB = 1024 * 1024;
        private const int GB = 1024 * MB;

        /// <summary>
        /// Timeout on post import for large files
        /// </summary>
        public int PostImportTimeoutInMinutes { get; set; } = 15;

        /// <summary>
        /// Uploads a PBIX file to the MyWorkspace
        /// </summary>
        /// <param name='file'>
        /// The PBIX file to import
        /// </param>
        /// <param name='datasetDisplayName'>
        /// The dataset display name
        /// </param>
        /// <param name='nameConflict'>
        /// Whether to overwrite dataset during conflicts
        /// </param>
        /// <param name="customHeaders">
        /// Optional custom headers
        /// </param>
        /// <param name="cancellationToken">
        /// Optional cancellation token
        /// </param>
        public Task<HttpOperationResponse<Import>> PostImportFileWithHttpMessage(Stream file, string datasetDisplayName = default, string nameConflict = default, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default) => PostImportFileWithHttpMessage(
                            groupId: null,
                            file: file,
                            datasetDisplayName: datasetDisplayName,
                            nameConflict: nameConflict,
                            customHeaders: customHeaders,
                            cancellationToken: cancellationToken);

        /// <summary>
        /// Uploads a PBIX file to the specified group
        /// </summary>
        /// <param name='groupId'>
        /// The group Id
        /// </param>
        /// <param name='file'>
        /// The PBIX file to import
        /// </param>
        /// <param name='datasetDisplayName'>
        /// The dataset display name
        /// </param>
        /// <param name='nameConflict'>
        /// Whether to overwrite dataset during conflicts
        /// </param>
        /// <param name="customHeaders">
        /// Optional custom headers
        /// </param>
        /// <param name="cancellationToken">
        /// Optional cancellation token
        /// </param>
        public Task<HttpOperationResponse<Import>> PostImportFileWithHttpMessage(string groupId, Stream file, string datasetDisplayName = default, string nameConflict = default, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default)
        {
            // Tracing
            string _invocationId = null;
            if (ServiceClientTracing.IsEnabled)
            {
                _invocationId = ServiceClientTracing.NextInvocationId.ToString();
                var tracingParameters = new Dictionary<string, object>
                {
                    { "groupId", groupId ?? string.Empty },
                    { "file", file },
                    { "datasetDisplayName", datasetDisplayName },
                    { "nameConflict", nameConflict },
                    { "cancellationToken", cancellationToken }
                };
                ServiceClientTracing.Enter(_invocationId, this, "PostImport", tracingParameters);
            }

            if (file == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "file");
            }

            if (string.IsNullOrEmpty(datasetDisplayName))
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "datasetDisplayName");
            }

            if (file.Length > 1 * GB)
            {
                return UploadLargeFile(groupId, file, datasetDisplayName, nameConflict, customHeaders, cancellationToken);
            }
            else
            {
                return UploadFile(groupId, file, datasetDisplayName, nameConflict, customHeaders, cancellationToken);
            }
        }

        private async Task<HttpOperationResponse<Import>> UploadFile(string groupId, Stream file, string datasetDisplayName = default, string nameConflict = default, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default)
        {
            // Tracing
            var _shouldTrace = ServiceClientTracing.IsEnabled;
            const string _invocationId = null;

            var groupsPart = (!string.IsNullOrEmpty(groupId)) ? "v1.0/myorg/groups/{groupId}/imports" : "v1.0/myorg/imports";
            var _baseUrl = Client.BaseUri.AbsoluteUri;
            var _url = new Uri(new Uri(_baseUrl + (_baseUrl.EndsWith("/") ? "" : "/")), groupsPart).ToString();

            if (!string.IsNullOrEmpty(groupId))
            {
                _url = _url.Replace("{groupId}", Uri.EscapeDataString(groupId));
            }

            var _queryParameters = new List<string>();
            if (datasetDisplayName != null)
            {
                _queryParameters.Add(string.Format("datasetDisplayName={0}", Uri.EscapeDataString(datasetDisplayName)));
            }
            if (nameConflict != null)
            {
                _queryParameters.Add(string.Format("nameConflict={0}", Uri.EscapeDataString(nameConflict)));
            }
            if (_queryParameters.Count > 0)
            {
                _url += "?" + string.Join("&", _queryParameters);
            }

            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            HttpResponseMessage _httpResponse = null;
            _httpRequest.Method = new HttpMethod("POST");
            _httpRequest.RequestUri = new Uri(_url);
            // Set Headers
            if (customHeaders != null)
            {
                foreach (var _header in customHeaders)
                {
                    if (_httpRequest.Headers.Contains(_header.Key))
                    {
                        _httpRequest.Headers.Remove(_header.Key);
                    }
                    _httpRequest.Headers.TryAddWithoutValidation(_header.Key, _header.Value);
                }
            }

            // Serialize Request
            var multiPartContent = new MultipartFormDataContent
            {
                new StreamContent(file)
            };

            _httpRequest.Content = multiPartContent;

            // Set Credentials
            if (Client.Credentials != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Client.Credentials.ProcessHttpRequestAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            }
            // Send Request
            if (_shouldTrace)
            {
                ServiceClientTracing.SendRequest(_invocationId, _httpRequest);
            }
            cancellationToken.ThrowIfCancellationRequested();
            _httpResponse = await Client.HttpClient.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
            if (_shouldTrace)
            {
                ServiceClientTracing.ReceiveResponse(_invocationId, _httpResponse);
            }
            var _statusCode = _httpResponse.StatusCode;
            cancellationToken.ThrowIfCancellationRequested();
            string _responseContent = null;
            if ((int)_statusCode != 202) // Accepted
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", _statusCode))
                {
                    Request = new HttpRequestMessageWrapper(_httpRequest, null),
                    Response = new HttpResponseMessageWrapper(_httpResponse, _responseContent)
                };
                if (_shouldTrace)
                {
                    ServiceClientTracing.Error(_invocationId, ex);
                }
                _httpRequest.Dispose();
                _httpResponse?.Dispose();
                throw ex;
            }
            // Create Result
            var _result = new HttpOperationResponse<Import>
            {
                Request = _httpRequest,
                Response = _httpResponse
            };
            // Deserialize Response
            if ((int)_statusCode == 202) // Accepted
            {
                _responseContent = await _httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    _result.Body = SafeJsonConvert.DeserializeObject<Import>(_responseContent, Client.DeserializationSettings);
                }
                catch (JsonException ex)
                {
                    _httpRequest.Dispose();
                    _httpResponse?.Dispose();
                    throw new SerializationException("Unable to deserialize the response.", _responseContent, ex);
                }
            }
            if (_shouldTrace)
            {
                ServiceClientTracing.Exit(_invocationId, _result);
            }
            return _result;
        }

        private async Task<HttpOperationResponse<Import>> UploadLargeFile(string groupId, Stream file, string datasetDisplayName = default, string nameConflict = default, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default)
        {
            TemporaryUploadLocation temporaryUploadLocation;

            if (groupId == null)
            {
                temporaryUploadLocation = Client.Imports.CreateTemporaryUploadLocation();
            }
            else
            {
                temporaryUploadLocation = Client.Imports.CreateTemporaryUploadLocationInGroup(groupId);
            }

            if (temporaryUploadLocation == null || string.IsNullOrEmpty(temporaryUploadLocation.Url))
            {
                throw new Exception("Create temporary upload location step failed");
            }

            await UploadFileToBlob(temporaryUploadLocation.Url, file, cancellationToken).ConfigureAwait(false);

            using (var powerBIClient = new PowerBIClient(Client.BaseUri, Client.Credentials))
            {
                powerBIClient.HttpClient.Timeout = new TimeSpan(hours: 0, minutes: PostImportTimeoutInMinutes, seconds: 0);

                if (Path.GetExtension(datasetDisplayName)?.Length == 0)
                {
                    datasetDisplayName = Path.GetFileNameWithoutExtension(datasetDisplayName) + ".pbix";
                }

                if (groupId == null)
                {
                    return await powerBIClient.Imports.PostImportWithHttpMessagesAsync(datasetDisplayName, new ImportInfo { FileUrl = temporaryUploadLocation.Url }, nameConflict, customHeaders, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return await powerBIClient.Imports.PostImportInGroupWithHttpMessagesAsync(groupId, datasetDisplayName, new ImportInfo { FileUrl = temporaryUploadLocation.Url }, nameConflict, customHeaders, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task UploadFileToBlob(string temporaryUploadLocationUrl, Stream file, CancellationToken cancellationToken)
        {
            const int maxBlockSize = 4 * MB;

            var blockIds = new List<string>();
            var buffer = new byte[maxBlockSize];
            var i = 1;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-ms-version", "2015-04-05");
                var baseUploadUrl = temporaryUploadLocationUrl + "&comp=block&blockid=";
                var headers = new Dictionary<string, string>
                {
                    { "x-ms-blob-type", "BlockBlob" }
                };
                do
                {
                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("block-" + i.ToString("000000"));
                    var blockId = Convert.ToBase64String(plainTextBytes);
                    var bytesRead = await file.ReadAsync(buffer, 0, maxBlockSize).ConfigureAwait(false);

                    var uploadBlockUrl = baseUploadUrl + blockId;
                    var content = new ByteArrayContent(buffer, 0, bytesRead);
                    await SendRequest(httpClient, HttpMethod.Put, uploadBlockUrl, headers, content, HttpStatusCode.Created, cancellationToken).ConfigureAwait(false);

                    blockIds.Add(blockId);
                    i++;
                }
                while (file.Length - file.Position > 0);

                var blockList = new BlockList { BlockIds = blockIds };

                var xsn = new XmlSerializerNamespaces();
                xsn.Add(string.Empty, string.Empty);

                var serializer = new XmlSerializer(typeof(BlockList));
                var myWriter = new UTF8StringWriter();
                serializer.Serialize(myWriter, blockList, xsn);
                var requestBody = myWriter.ToString();

                headers.Clear();
                headers.Add("x-ms-blob-content-type", "application/x-zip-compressed");
                var uploadBlocListkUrl = temporaryUploadLocationUrl + "&comp=blocklist";
                await SendRequest(httpClient, HttpMethod.Put, uploadBlocListkUrl, headers, new StringContent(requestBody), HttpStatusCode.Created, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task SendRequest(HttpClient client, HttpMethod method, string url, Dictionary<string, string> headers, HttpContent content, HttpStatusCode expectedHttpStatus, CancellationToken cancellationToken)
        {
            using (var request = new HttpRequestMessage(method, url))
            {
                request.Content = content;

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                // Send Request
                if (ServiceClientTracing.IsEnabled)
                {
                    ServiceClientTracing.SendRequest(null, request);
                }
                cancellationToken.ThrowIfCancellationRequested();
                using (var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    if (ServiceClientTracing.IsEnabled)
                    {
                        ServiceClientTracing.ReceiveResponse(null, response);
                    }
                    cancellationToken.ThrowIfCancellationRequested();

                    await VerifyStatusCode(response, expectedHttpStatus).ConfigureAwait(false);
                }
            }
        }

        private async Task VerifyStatusCode(HttpResponseMessage responseMessage, HttpStatusCode statusCode)
        {
            if (responseMessage.StatusCode != statusCode)
            {
                var ex = new HttpOperationException(string.Format("Operation returned an invalid status code '{0}'", responseMessage.StatusCode));

                var responseContent = string.Empty;
                if (responseMessage.Content != null)
                {
                    responseContent = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                ex.Request = new HttpRequestMessageWrapper(responseMessage.RequestMessage, null);
                ex.Response = new HttpResponseMessageWrapper(responseMessage, responseContent);

                if (ServiceClientTracing.IsEnabled)
                {
                    ServiceClientTracing.Error(null, ex);
                }

                throw ex;
            }
        }

        private class UTF8StringWriter : StringWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }
        }

        public class BlockList
        {
            [XmlElement(ElementName = "Latest")]
            public List<string> BlockIds { get; set; }
        }
    }
}