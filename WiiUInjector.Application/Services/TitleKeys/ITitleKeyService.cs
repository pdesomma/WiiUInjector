using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Services.TitleKeys
{
    /// <summary>
    /// Service for querying and managing persistent title keys.
    /// Delegates to ITitleKeyStore and provides logging and input validation.
    /// </summary>
    public interface ITitleKeyService
    {
        /// <summary>
        /// Retrieves a title key by console, title ID, and optional region.
        /// </summary>
        /// <param name="console">The console type.</param>
        /// <param name="titleId">The Nintendo Title ID; must not be null or whitespace.</param>
        /// <param name="region">The region; may be null for region-less bases.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the TitleKey on success, or errors (including not-found) on failure.</returns>
        Task<Result<TitleKey>> GetAsync(ConsoleType console, string titleId, Region? region, CancellationToken cancellationToken);

        /// <summary>
        /// Lists all title keys for a specified console.
        /// </summary>
        /// <param name="console">The console type to query.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the list of TitleKey entries on success, or errors on failure.</returns>
        Task<Result<IReadOnlyList<TitleKey>>> ListAsync(ConsoleType console, CancellationToken cancellationToken);

        /// <summary>
        /// Persists a title key (insert or update).
        /// </summary>
        /// <param name="key">The TitleKey to save; must not be null.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result indicating success or any validation/persistence errors.</returns>
        Task<Result> SaveAsync(TitleKey key, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes a title key by console, title ID, and optional region.
        /// </summary>
        /// <param name="console">The console type.</param>
        /// <param name="titleId">The Nintendo Title ID; must not be null or whitespace.</param>
        /// <param name="region">The region; may be null for region-less bases.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result indicating success or any errors (e.g., not-found).</returns>
        Task<Result> DeleteAsync(ConsoleType console, string titleId, Region? region, CancellationToken cancellationToken);
    }
}
