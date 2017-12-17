using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SceneSkope.PowerBI
{
    public class PowerBIClientException : Exception
    {
        private static string CreateMessage(HttpStatusCode statusCode, string reasonPhrase, string response)
        {
            if (string.IsNullOrWhiteSpace(response))
            {
                return $"{statusCode} {reasonPhrase}";
            }
            else
            {
                return $"{statusCode} {response}";
            }
        }

        public HttpStatusCode StatusCode { get; }
        public string ReasonPhrase { get; }
        public string Response { get; }

        public PowerBIClientException(HttpStatusCode statusCode, string reasonPhrase, string response)
            : base(CreateMessage(statusCode, reasonPhrase, response))
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            Response = response;
        }

        public PowerBIClientException() : base()
        {
        }

        protected PowerBIClientException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public PowerBIClientException(string message) : base(message)
        {
        }

        public PowerBIClientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
