using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace SceneSkope.PowerBI.Authenticators
{
    public abstract class BaseAdalTokenProvider : ITokenProvider
    {
        private static readonly TimeSpan ExpirationThreshold = TimeSpan.FromMinutes(5);

        public string AuthorityUri { get; set; } = "https://login.windows.net/common/oauth2/authorize";

        public string ResourceUri { get; set; } = "https://analysis.windows.net/powerbi/api";

        protected AuthenticationContext AuthenticationContext { get; }
        public string ClientId { get; }

        public AuthenticationResult LatestResult { get; private set; }

        protected BaseAdalTokenProvider(string clientId, byte[] initialState = null)
        {
            var tokenCache = (initialState == null) ? new TokenCache() : new TokenCache(initialState);
            AuthenticationContext = new AuthenticationContext(AuthorityUri, tokenCache);
            ClientId = clientId;
        }

        public async Task<AuthenticationResult> GetAccessTokenAsync(CancellationToken ct)
        {
            if (HasExpired())
            {
                LatestResult = null;
            }
            if (LatestResult == null)
            {
                if (AuthenticationContext.TokenCache.Count > 0)
                {
                    var result = await AuthenticationContext.AcquireTokenSilentAsync(ResourceUri, ClientId).ConfigureAwait(false);
                    LatestResult = result;
                    return result;
                }
            }

            if (LatestResult == null)
            {
                var result = await InitialGetAccessCodeAsync(ct).ConfigureAwait(false);
                LatestResult = result;
                return result;
            }
            else
            {
                return LatestResult;
            }
        }

        public bool HasExpired() => (LatestResult == null) || (DateTime.UtcNow + ExpirationThreshold >= LatestResult.ExpiresOn);

        protected abstract Task<AuthenticationResult> InitialGetAccessCodeAsync(CancellationToken ct);

        public byte[] GetSerializedState()
        {
            var state = AuthenticationContext.TokenCache.Serialize();
            AuthenticationContext.TokenCache.HasStateChanged = false;
            return state;
        }

        public bool HasStateChanged => AuthenticationContext.TokenCache.HasStateChanged;

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            var token = await GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
            return new AuthenticationHeaderValue(LatestResult.AccessTokenType, LatestResult.AccessToken);
        }
    }
}
