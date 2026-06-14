using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Abstractions.Settings
{
    /// <summary>
    /// Interface for persisting and retrieving application settings.
    /// </summary>
    public interface IApplicationSettingsStore
    {
        /// <summary>
        /// Loads application settings asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token for cancelling the operation.</param>
        /// <returns>
        /// A Result containing the loaded ApplicationSettings if successful, or an error result if the operation fails.
        /// </returns>
        Task<Result<ApplicationSettings>> LoadAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Saves the specified application settings asynchronously.
        /// </summary>
        /// <param name="settings">The application settings to save.</param>
        /// <param name="cancellationToken">A token for cancelling the operation.</param>
        /// <returns>
        /// A Result indicating success or containing errors if the operation fails.
        /// </returns>
        Task<Result> SaveAsync(ApplicationSettings settings, CancellationToken cancellationToken);
    }
}
