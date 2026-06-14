using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Abstractions.IO
{
    /// <summary>
    /// Loads a ROM file and returns an IRomSource with metadata.
    /// </summary>
    public interface IRomFileLoader
    {
        /// <summary>
        /// Loads a ROM file asynchronously from the given file path.
        /// </summary>
        /// <param name="filePath">Absolute or relative path to the ROM file.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the loaded ROM and its file name on success, or validation errors on failure.</returns>
        Task<Result<RomLoadResult>> LoadAsync(string filePath, CancellationToken cancellationToken);
    }
}
