// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace SceneSkope.PowerBI
{
    using Models;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for Users.
    /// </summary>
    public static partial class UsersExtensions
    {
            /// <summary>
            /// Refreshes user permissions in Power BI
            /// </summary>
            /// <remarks>
            /// When a user is granted permissions to a workspace, app, or artifact, it
            /// might not be immediately available through API calls. &lt;br/&gt;This
            /// operation refreshes user permissions and makes sure the user permissions
            /// are fully updated.  &lt;br/&gt;&lt;br/&gt;**Required scope**:
            /// Workspace.Read.All  or Workspace.ReadWrite.All&lt;br/&gt;To set the
            /// permissions scope, see [Register an
            /// app](https://docs.microsoft.com/power-bi/developer/register-app).
            /// &lt;h2&gt;Restrictions&lt;/h2&gt; User can call this API once per hour.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static void RefreshUserPermissions(this IUsers operations)
            {
                operations.RefreshUserPermissionsAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// Refreshes user permissions in Power BI
            /// </summary>
            /// <remarks>
            /// When a user is granted permissions to a workspace, app, or artifact, it
            /// might not be immediately available through API calls. &lt;br/&gt;This
            /// operation refreshes user permissions and makes sure the user permissions
            /// are fully updated.  &lt;br/&gt;&lt;br/&gt;**Required scope**:
            /// Workspace.Read.All  or Workspace.ReadWrite.All&lt;br/&gt;To set the
            /// permissions scope, see [Register an
            /// app](https://docs.microsoft.com/power-bi/developer/register-app).
            /// &lt;h2&gt;Restrictions&lt;/h2&gt; User can call this API once per hour.
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task RefreshUserPermissionsAsync(this IUsers operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.RefreshUserPermissionsWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

    }
}
