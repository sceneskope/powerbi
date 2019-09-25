using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Rest;

namespace SceneSkope.PowerBI.Authenticators
{
    public abstract class BaseAdalTokenProvider : ITokenProvider
    {
        public IPublicClientApplication App { get; }

        private static readonly TimeSpan ExpirationThreshold = TimeSpan.FromMinutes(5);

        public string AuthorityUri { get; set; } = "https://login.windows.net/common/oauth2/authorize";

        public string ResourceUri { get; set; } = "https://analysis.windows.net/powerbi/api";

        public string[] Scopes { get; set; } = new[] { "User.Read" };
        public string[] PowerBIScopes { get; set; } = new[] { "Workspace.Read.All" };

        public string ClientId { get; }

        public AuthenticationResult LatestResult { get; private set; }

        protected BaseAdalTokenProvider(string clientId, byte[] initialState = null)
        {
            App = PublicClientApplicationBuilder.Create(clientId).Build();
            var tokenCache = App.UserTokenCache;
            if (initialState != null)
            {
                tokenCache.SetBeforeAccess(args => args.TokenCache.DeserializeMsalV3(initialState));
            }
            tokenCache.SetAfterAccess(args =>
            {
                if (args.HasStateChanged)
                {
                    _serializedState = args.TokenCache.SerializeMsalV3();
                    HasStateChanged = true;
                }
            });
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
                var allScopes = Scopes.Concat(PowerBIScopes.Select(s => ResourceUri + "/" + s));
                var accounts = await App.GetAccountsAsync().ConfigureAwait(false);
                try
                {
                    var result = await App.AcquireTokenSilent(allScopes, accounts.FirstOrDefault())
                        .ExecuteAsync().ConfigureAwait(false);
                    LatestResult = result;
                    return result;
                }
                catch (MsalUiRequiredException)
                {
                    var result = await InitialGetAccessCodeAsync(ct).ConfigureAwait(false);
                    LatestResult = result;
                    return result;
                }
            }
            return LatestResult;
        }

        public bool HasExpired() => (LatestResult == null) || (DateTime.UtcNow + ExpirationThreshold >= LatestResult.ExpiresOn);

        protected abstract Task<AuthenticationResult> InitialGetAccessCodeAsync(CancellationToken ct);

        public byte[] GetSerializedState()
        {
            HasStateChanged = false;
            return _serializedState;
        }

        private byte[] _serializedState;

        public bool HasStateChanged { get; private set; }

        public async Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(CancellationToken cancellationToken)
        {
            var token = await GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
            return new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }
    }
}
