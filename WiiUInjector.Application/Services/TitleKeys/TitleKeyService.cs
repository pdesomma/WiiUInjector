using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Logging;
using WiiUInjector.Application.Abstractions.TitleKeys;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Services.TitleKeys
{
    /// <summary>
    /// Implementation of ITitleKeyService.
    /// Delegates to ITitleKeyStore and provides logging and input validation at the service boundary.
    /// </summary>
    public sealed class TitleKeyService : ITitleKeyService
    {
        /// <summary>
        /// Store for title key persistence.
        /// </summary>
        private readonly ITitleKeyStore _store;

        /// <summary>
        /// Logger for service operations.
        /// </summary>
        private readonly IApplicationLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleKeyService"/> class.
        /// </summary>
        /// <param name="store">The title key store; must not be null.</param>
        /// <param name="logger">The application logger; must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown if store or logger is null.</exception>
        public TitleKeyService(ITitleKeyStore store, IApplicationLogger logger)
        {
            if (store == null)
                throw new ArgumentNullException(nameof(store));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _store = store;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a title key by console, title ID, and optional region.
        /// Logs success (Info), not-found (Info), or other failure (Warn).
        /// </summary>
        /// <param name="console">The console type.</param>
        /// <param name="titleId">The Nintendo Title ID; must not be null or whitespace.</param>
        /// <param name="region">The region; may be null for region-less bases.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the TitleKey on success, or errors (including not-found) on failure.</returns>
        public async Task<Result<TitleKey>> GetAsync(ConsoleType console, string titleId, Region? region, CancellationToken cancellationToken)
        {
            var result = await _store.GetAsync(console, titleId, region, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.Info($"Retrieved title key for {console}/{titleId}/{region}.");
                return result;
            }

            // Check if this is a not-found error (expected case).
            if (IsNotFoundError(result))
            {
                _logger.Info($"No title key found for {console}/{titleId}/{region}.");
                return result;
            }

            // Log other failures as warnings.
            LogFailure(result);
            return result;
        }

        /// <summary>
        /// Lists all title keys for a specified console.
        /// Logs success or failure.
        /// </summary>
        /// <param name="console">The console type to query.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the list of TitleKey entries on success, or errors on failure.</returns>
        public async Task<Result<IReadOnlyList<TitleKey>>> ListAsync(ConsoleType console, CancellationToken cancellationToken)
        {
            var result = await _store.ListAsync(console, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.Info($"Listed {result.Value.Count} title key(s) for {console}.");
                return result;
            }

            LogFailure(result);
            return result;
        }

        /// <summary>
        /// Persists a title key (insert or update).
        /// Validates input before delegating to store. Logs success or failure.
        /// </summary>
        /// <param name="key">The TitleKey to save; must not be null.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result indicating success or any validation/persistence errors.</returns>
        public async Task<Result> SaveAsync(TitleKey key, CancellationToken cancellationToken)
        {
            // Validate input at the service boundary.
            if (key == null)
            {
                var error = new ApplicationError(
                    ApplicationErrors.CustomBaseRomInvalid,
                    "key cannot be null.",
                    nameof(key));

                return Result.Failure(error);
            }

            var result = await _store.SaveAsync(key, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.Info($"Saved title key for {key.Console}/{key.TitleId}/{key.Region}.");
                return result;
            }

            LogFailure(result);
            return result;
        }

        /// <summary>
        /// Deletes a title key by console, title ID, and optional region.
        /// Logs success or failure.
        /// </summary>
        /// <param name="console">The console type.</param>
        /// <param name="titleId">The Nintendo Title ID; must not be null or whitespace.</param>
        /// <param name="region">The region; may be null for region-less bases.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result indicating success or any errors (e.g., not-found).</returns>
        public async Task<Result> DeleteAsync(ConsoleType console, string titleId, Region? region, CancellationToken cancellationToken)
        {
            // Validate input at the service boundary.
            if (string.IsNullOrWhiteSpace(titleId))
            {
                var error = new ApplicationError(
                    ApplicationErrors.CustomBaseRomInvalid,
                    "titleId cannot be null or empty.",
                    nameof(titleId));

                return Result.Failure(error);
            }

            var result = await _store.DeleteAsync(console, titleId, region, cancellationToken);

            if (result.IsSuccess)
            {
                _logger.Info($"Deleted title key for {console}/{titleId}/{region}.");
                return result;
            }

            LogFailure(result);
            return result;
        }

        /// <summary>
        /// Checks if a failure result contains a not-found error code.
        /// </summary>
        private static bool IsNotFoundError(Result result)
        {
            foreach (var error in result.Errors)
            {
                if (string.Equals(error.Code, ApplicationErrors.TitleKeyNotFound, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
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
