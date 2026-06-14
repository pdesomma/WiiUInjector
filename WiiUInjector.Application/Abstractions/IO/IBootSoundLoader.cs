using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Abstractions.IO
{
    /// <summary>
    /// Loads a boot sound file and returns a materialized BootSound value object.
    /// </summary>
    public interface IBootSoundLoader
    {
        /// <summary>
        /// Loads a boot sound file asynchronously and materializes it into a BootSound value object.
        /// </summary>
        /// <param name="filePath">Absolute or relative path to the boot sound file.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the materialized BootSound on success, or validation errors on failure.</returns>
        Task<Result<BootSound>> LoadAsync(string filePath, CancellationToken cancellationToken);
    }
}
