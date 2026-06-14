using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Abstractions.Updates
{
    /// <summary>
    /// Interface for checking application updates.
    /// </summary>
    public interface IUpdateChecker
    {
        /// <summary>
        /// Checks for available updates asynchronously.
        /// </summary>
        /// <param name="currentVersion">The current application version.</param>
        /// <param name="cancellationToken">A token for cancelling the operation.</param>
        /// <returns>
        /// A Result containing UpdateInfo if the check succeeds, or an error result if the operation fails.
        /// </returns>
        Task<Result<UpdateInfo>> CheckAsync(string currentVersion, CancellationToken cancellationToken);
    }
}
