using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Updates;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Services.Updates
{
    /// <summary>
    /// Service for checking application updates.
    /// </summary>
    public interface IUpdateService
    {
        /// <summary>
        /// Checks for available application updates.
        /// </summary>
        /// <param name="currentVersion">The current application version; must not be null or whitespace.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing UpdateInfo on success, or errors on failure.</returns>
        Task<Result<UpdateInfo>> CheckForUpdatesAsync(string currentVersion, CancellationToken cancellationToken);
    }
}
