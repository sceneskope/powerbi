using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SceneSkope.PowerBI.Authenticators
{
    public abstract class BaseAdalAuthenticator : IAuthenticator
    {
        public string AuthorityUri { get; set; } = "https://login.windows.net/common/oauth2/authorize";

        public string ResourceUri { get; set; } = "https://analysis.windows.net/powerbi/api";

        protected AuthenticationContext AuthenticationContext { get; }
        protected string ClientId { get; }

        protected BaseAdalAuthenticator(string clientId, byte[] initialState = null)
        {
            var tokenCache = (initialState == null) ? new TokenCache() : new TokenCache(initialState);
            AuthenticationContext = new AuthenticationContext(AuthorityUri, tokenCache);
            ClientId = clientId;
        }

        private string _accessToken;

        public async Task<string> GetAccessTokenAsync(CancellationToken ct)
        {
            if (_accessToken == null)
            {
                if (AuthenticationContext.TokenCache.Count > 0)
                {
                    var result = await AuthenticationContext.AcquireTokenSilentAsync(ResourceUri, ClientId).ConfigureAwait(false);
                    _accessToken = result.AccessToken;
                    return _accessToken;
                }
            }

            if (_accessToken == null)
            {
                _accessToken = await InitialGetAccessCodeAsync(ct).ConfigureAwait(false);
            }
            else
            {
                var result = await AuthenticationContext.AcquireTokenSilentAsync(ResourceUri, ClientId).ConfigureAwait(false);
                _accessToken = result.AccessToken;
            }

            return _accessToken;
        }

        protected abstract Task<string> InitialGetAccessCodeAsync(CancellationToken ct);

        public byte[] GetSerializedState()
        {
            var state = AuthenticationContext.TokenCache.Serialize();
            AuthenticationContext.TokenCache.HasStateChanged = false;
            return state;
        }

        public bool HasStateChanged => AuthenticationContext.TokenCache.HasStateChanged;
    }
}
