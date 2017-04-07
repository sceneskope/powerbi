using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SceneSkope.PowerBI
{
    public interface IAuthenticator
    {
        Task<string> GetAccessTokenAsync(CancellationToken ct);
    }
}
