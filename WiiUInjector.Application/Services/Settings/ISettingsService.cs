using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Settings;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Services.Settings
{
    /// <summary>
    /// Service for loading and saving application settings.
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Loads application settings asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the loaded ApplicationSettings on success, or errors on failure.</returns>
        Task<Result<ApplicationSettings>> LoadAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Saves the specified application settings asynchronously.
        /// </summary>
        /// <param name="settings">The application settings to save; must not be null.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result indicating success or containing save errors.</returns>
        Task<Result> SaveAsync(ApplicationSettings settings, CancellationToken cancellationToken);
    }
}
