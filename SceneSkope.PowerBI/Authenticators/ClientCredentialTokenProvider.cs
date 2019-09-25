using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace SceneSkope.PowerBI.Authenticators
{
    public class ClientCredentialTokenProvider : BaseAdalTokenProvider
    {
        public ClientCredentialTokenProvider(string clientId, string secret) : base(clientId)
        {
            Application = ConfidentialClientApplicationBuilder.Create(clientId).WithClientSecret(secret).Build();
        }

        public IConfidentialClientApplication Application { get; }

        protected override async Task<AuthenticationResult> InitialGetAccessCodeAsync(CancellationToken ct)
        {
            var accounts = await Application.GetAccountsAsync().ConfigureAwait(false);
            var account = accounts.FirstOrDefault();
            return await App.AcquireTokenSilent(Scopes, account).ExecuteAsync(ct).ConfigureAwait(false);
        }
    }
}
