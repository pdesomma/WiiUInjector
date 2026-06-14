using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Abstractions.IO
{
    /// <summary>
    /// Loads an image file and returns a materialized GameImage value object.
    /// </summary>
    public interface IGameImageLoader
    {
        /// <summary>
        /// Loads an image file asynchronously and materializes it into a GameImage value object.
        /// </summary>
        /// <param name="filePath">Absolute or relative path to the image file.</param>
        /// <param name="slot">The image slot (Icon, Drc, Tv, or Logo) this image occupies.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the materialized GameImage on success, or validation errors on failure.</returns>
        Task<Result<GameImage>> LoadAsync(string filePath, ImageSlot slot, CancellationToken cancellationToken);
    }
}
