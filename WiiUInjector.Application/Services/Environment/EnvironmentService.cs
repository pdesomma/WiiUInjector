using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Environment;
using WiiUInjector.Application.Abstractions.Logging;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Services.Environment
{
    /// <summary>
    /// Implementation of IEnvironmentService.
    /// Delegates to IEnvironmentInspector and provides logging at the service boundary.
    /// </summary>
    public sealed class EnvironmentService : IEnvironmentService
    {
        /// <summary>
        /// Inspector for system environment conditions.
        /// </summary>
        private readonly IEnvironmentInspector _inspector;

        /// <summary>
        /// Logger for service operations.
        /// </summary>
        private readonly IApplicationLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentService"/> class.
        /// </summary>
        /// <param name="inspector">The environment inspector; must not be null.</param>
        /// <param name="logger">The application logger; must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if inspector or logger is null.</exception>
        public EnvironmentService(IEnvironmentInspector inspector, IApplicationLogger logger)
        {
            if (inspector == null)
                throw new ArgumentNullException(nameof(inspector));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _inspector = inspector;
            _logger = logger;
        }

        /// <summary>
        /// Inspects the current system environment and returns a report.
        /// Delegates to the inspector and logs the result with environment details.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the EnvironmentReport on success, or errors on failure.</returns>
        public async Task<Result<EnvironmentReport>> InspectAsync(CancellationToken cancellationToken)
        {
            var result = await _inspector.InspectAsync(cancellationToken);

            if (result.IsSuccess)
            {
                var report = result.Value;
                var logMessage = $"Env: OneDrive={report.IsRunningFromOneDrive} Wine={report.IsRunningUnderWineOrSimilar} Resolution={report.ScreenWidth}x{report.ScreenHeight}";
                _logger.Info(logMessage);
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
