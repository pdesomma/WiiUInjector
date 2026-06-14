using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Services.BaseRoms
{
    /// <summary>
    /// Service for querying and loading base ROMs (curated and custom).
    /// Delegates to IBaseRomCatalog and logs operations.
    /// </summary>
    public interface IBaseRomService
    {
        /// <summary>
        /// Lists all curated base ROMs for the specified console.
        /// </summary>
        /// <param name="console">The console type to list base ROMs for.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the list of BaseRom entries on success, or errors on failure.</returns>
        Task<Result<IReadOnlyList<BaseRom>>> ListAsync(ConsoleType console, CancellationToken cancellationToken);

        /// <summary>
        /// Finds a curated base ROM by console, title ID, and optional region.
        /// </summary>
        /// <param name="console">The console type to query.</param>
        /// <param name="titleId">The Nintendo Title ID.</param>
        /// <param name="region">The region; may be null for region-less bases.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the matching BaseRom on success, or errors (including not-found) on failure.</returns>
        Task<Result<BaseRom>> FindCuratedAsync(ConsoleType console, string titleId, Region? region, CancellationToken cancellationToken);

        /// <summary>
        /// Loads a custom base ROM from a local file path.
        /// </summary>
        /// <param name="console">The console type this base targets.</param>
        /// <param name="customPath">Absolute or relative path to the custom base ROM file; must not be null or whitespace.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the loaded custom BaseRom on success, or errors on failure.</returns>
        Task<Result<BaseRom>> LoadCustomAsync(ConsoleType console, string customPath, CancellationToken cancellationToken);
    }
}
