using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace SceneSkope.PowerBI.Authenticators
{
    public class DeviceCodeTokenProvider : BaseAdalTokenProvider
    {
        private readonly Action<string, string> _notifyDeviceCodeRequest;

        public DeviceCodeTokenProvider(string clientId, Action<string, string> notifyDeviceCodeRequest)
            : base(clientId)
        {
            _notifyDeviceCodeRequest = notifyDeviceCodeRequest;
        }

        public DeviceCodeTokenProvider(string clientId, byte[] initialState) : base(clientId, initialState)
        {
#pragma warning disable RCS1163 // Unused parameter.
            _notifyDeviceCodeRequest = (uri, code) => throw new InvalidOperationException("Device code authentication failed");
#pragma warning restore RCS1163 // Unused parameter.
        }

        protected override async Task<AuthenticationResult> InitialGetAccessCodeAsync(CancellationToken ct)
        {
            var codeResult = await App.AcquireTokenWithDeviceCode(Scopes, dcr =>
            {
                _notifyDeviceCodeRequest(dcr.VerificationUrl, dcr.UserCode);
                return Task.CompletedTask;
            }).ExecuteAsync(ct).ConfigureAwait(false);
            return codeResult;
        }
    }
}
