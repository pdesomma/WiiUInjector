using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Logging;
using WiiUInjector.Application.Abstractions.Updates;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Services.Updates
{
    /// <summary>
    /// Implementation of IUpdateService.
    /// Delegates to IUpdateChecker and provides logging at the service boundary.
    /// </summary>
    public sealed class UpdateService : IUpdateService
    {
        /// <summary>
        /// Checker for application updates.
        /// </summary>
        private readonly IUpdateChecker _checker;

        /// <summary>
        /// Logger for service operations.
        /// </summary>
        private readonly IApplicationLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateService"/> class.
        /// </summary>
        /// <param name="checker">The update checker; must not be null.</param>
        /// <param name="logger">The application logger; must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if checker or logger is null.</exception>
        public UpdateService(IUpdateChecker checker, IApplicationLogger logger)
        {
            if (checker == null)
                throw new ArgumentNullException(nameof(checker));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _checker = checker;
            _logger = logger;
        }

        /// <summary>
        /// Checks for available application updates.
        /// Validates input before delegating to the checker and logs the result.
        /// </summary>
        /// <param name="currentVersion">The current application version; must not be null or whitespace.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing UpdateInfo on success, or errors on failure.</returns>
        public async Task<Result<UpdateInfo>> CheckForUpdatesAsync(string currentVersion, CancellationToken cancellationToken)
        {
            // Validate input at the service boundary.
            if (string.IsNullOrWhiteSpace(currentVersion))
            {
                var error = new ApplicationError(
                    ApplicationErrors.UpdateCheckFailed,
                    "currentVersion cannot be null or whitespace",
                    nameof(currentVersion));

                return Result<UpdateInfo>.Failure(error);
            }

            var result = await _checker.CheckAsync(currentVersion, cancellationToken);

            if (result.IsSuccess)
            {
                var updateInfo = result.Value;
                var updateStatus = updateInfo.IsNewerAvailable ? "newer version available" : "up to date";
                _logger.Info($"Update check completed: current={updateInfo.CurrentVersion}, latest={updateInfo.LatestVersion}, {updateStatus}");
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
