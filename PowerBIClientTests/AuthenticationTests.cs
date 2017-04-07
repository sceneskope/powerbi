using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SceneSkope.PowerBI.Authenticators;
using Xunit;

namespace PowerBIClientTests
{
    public class AuthenticationTests : IClassFixture<TestContext>
    {
        private readonly TestContext _context;

        public AuthenticationTests(TestContext context)
        {
            _context = context;
        }

        [Fact(Skip = "Not yet implemented")]
        public async Task VerifyUserLoginWorks()
        {
            Assert.False(string.IsNullOrWhiteSpace(_context.ClientConfiguration.UserName), "Configuration needs a user name");
            var authenticator = new UsernameAuthenticator(_context.ClientConfiguration.ClientId, _context.ClientConfiguration.UserName);
            var token = await authenticator.GetAccessTokenAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public async Task VerifyDeviceCodeAuthenticationWorks()
        {
            var authenticator = new DeviceCodeAuthenticator(_context.ClientConfiguration.ClientId, _context.ClientConfiguration.TokenCacheState);
            var token = await authenticator.GetAccessTokenAsync(CancellationToken.None).ConfigureAwait(false);
            Assert.False(string.IsNullOrWhiteSpace(token));
        }
    }
}
