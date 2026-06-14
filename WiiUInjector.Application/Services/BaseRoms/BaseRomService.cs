using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Catalog;
using WiiUInjector.Application.Abstractions.Logging;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Services.BaseRoms
{
    /// <summary>
    /// Implementation of IBaseRomService.
    /// Delegates to IBaseRomCatalog and provides logging and input validation at the service boundary.
    /// </summary>
    public sealed class BaseRomService : IBaseRomService
    {
        /// <summary>
        /// Catalog for curated and custom base ROMs.
        /// </summary>
        private readonly IBaseRomCatalog _catalog;

        /// <summary>
        /// Logger for service operations.
        /// </summary>
        private readonly IApplicationLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRomService"/> class.
        /// </summary>
        /// <param name="catalog">The base ROM catalog; must not be null.</param>
        /// <param name="logger">The application logger; must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if catalog or logger is null.</exception>
        public BaseRomService(IBaseRomCatalog catalog, IApplicationLogger logger)
        {
            if (catalog == null)
                throw new ArgumentNullException(nameof(catalog));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _catalog = catalog;
            _logger = logger;
        }

        /// <summary>
        /// Lists all curated base ROMs for the specified console.
        /// Logs success or failure.
        /// </summary>
        /// <param name="console">The console type to list base ROMs for.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the list of BaseRom entries on success, or errors on failure.</returns>
        public async Task<Result<IReadOnlyList<BaseRom>>> ListAsync(ConsoleType console, CancellationToken cancellationToken)
        {
            var result = await _catalog.ListCuratedAsync(console, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.Info($"Listed {result.Value.Count} curated base ROM(s) for {console}.");
                return result;
            }

            LogFailure(result);
            return result;
        }

        /// <summary>
        /// Finds a curated base ROM by console, title ID, and optional region.
        /// Logs success or failure.
        /// </summary>
        /// <param name="console">The console type to query.</param>
        /// <param name="titleId">The Nintendo Title ID.</param>
        /// <param name="region">The region; may be null for region-less bases.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the matching BaseRom on success, or errors (including not-found) on failure.</returns>
        public async Task<Result<BaseRom>> FindCuratedAsync(ConsoleType console, string titleId, Region? region, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(titleId))
            {
                return Result<BaseRom>.Failure(new ApplicationError(
                    ApplicationErrors.BaseRomNotInCatalog,
                    "titleId cannot be null or empty.",
                    nameof(titleId)));
            }

            var result = await _catalog.FindCuratedAsync(console, titleId, region, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.Info($"Found curated base ROM for {console}/{titleId}/{region}.");
                return result;
            }

            LogFailure(result);
            return result;
        }

        /// <summary>
        /// Loads a custom base ROM from a local file path.
        /// Validates input before delegating to catalog. Logs success or failure.
        /// </summary>
        /// <param name="console">The console type this base targets.</param>
        /// <param name="customPath">Absolute or relative path to the custom base ROM file; must not be null or whitespace.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the loaded custom BaseRom on success, or errors on failure.</returns>
        public async Task<Result<BaseRom>> LoadCustomAsync(ConsoleType console, string customPath, CancellationToken cancellationToken)
        {
            // Validate input at the service boundary.
            if (string.IsNullOrWhiteSpace(customPath))
            {
                var error = new ApplicationError(
                    ApplicationErrors.CustomBaseRomInvalid,
                    "customPath cannot be null or empty.",
                    nameof(customPath));

                return Result<BaseRom>.Failure(error);
            }

            var result = await _catalog.LoadCustomAsync(console, customPath, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.Info($"Loaded custom base ROM from {customPath} for {console}.");
                return result;
            }

            LogFailure(result);
            return result;
        }

        /// <summary>
        /// Logs the failure with code and message.
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
