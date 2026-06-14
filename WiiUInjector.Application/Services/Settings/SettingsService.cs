using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Logging;
using WiiUInjector.Application.Abstractions.Settings;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Services.Settings
{
    /// <summary>
    /// Implementation of ISettingsService.
    /// Delegates to IApplicationSettingsStore and provides logging at the service boundary.
    /// </summary>
    public sealed class SettingsService : ISettingsService
    {
        /// <summary>
        /// Store for application settings persistence.
        /// </summary>
        private readonly IApplicationSettingsStore _store;

        /// <summary>
        /// Logger for service operations.
        /// </summary>
        private readonly IApplicationLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsService"/> class.
        /// </summary>
        /// <param name="store">The application settings store; must not be null.</param>
        /// <param name="logger">The application logger; must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if store or logger is null.</exception>
        public SettingsService(IApplicationSettingsStore store, IApplicationLogger logger)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _store = store;
            _logger = logger;
        }

        /// <summary>
        /// Loads application settings asynchronously.
        /// Delegates to the store and logs the result.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the loaded ApplicationSettings on success, or errors on failure.</returns>
        public async Task<Result<ApplicationSettings>> LoadAsync(CancellationToken cancellationToken)
        {
            var result = await _store.LoadAsync(cancellationToken);

            if (result.IsSuccess)
            {
                _logger.Info("Application settings loaded successfully.");
                return result;
            }

            LogFailure(result);
            return result;
        }

        /// <summary>
        /// Saves the specified application settings asynchronously.
        /// Validates input before delegating to the store and logs the result.
        /// </summary>
        /// <param name="settings">The application settings to save; must not be null.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result indicating success or containing save errors.</returns>
        public async Task<Result> SaveAsync(ApplicationSettings settings, CancellationToken cancellationToken)
        {
            // Validate input at the service boundary.
            if (settings == null)
            {
                var error = new ApplicationError(
                    ApplicationErrors.SettingsSaveFailed,
                    "settings cannot be null",
                    nameof(settings));

                return Result.Failure(error);
            }

            var result = await _store.SaveAsync(settings, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.Info("Application settings saved successfully.");
                return result;
            }

            LogFailure(result);
            return result;
        }

        /// <summary>
        /// Logs all errors from a failed result.
        /// </summary>
        private void LogFailure(Result result)
        {
            foreach (var error in result.Errors)
            {
                _logger.Warn($"{error.Code}: {error.Message}");
            }
        }
    }
}
