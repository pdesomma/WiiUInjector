using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Abstractions.Operations
{
    /// <summary>
    /// Abstraction for transcoding ROM disc images between different formats.
    /// </summary>
    public interface IRomTranscoder
    {
        /// <summary>
        /// Transcodes a source disc image to the specified target format.
        /// The transcoded output is written to the workspace.
        /// </summary>
        /// <param name="sourcePath">Absolute path to the source disc image file.</param>
        /// <param name="targetFormat">The desired output format.</param>
        /// <param name="workspace">The injection workspace where the transcoded file will be written.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A Result containing the absolute path to the transcoded file on success, or errors on failure.</returns>
        Task<Result<string>> TranscodeAsync(string sourcePath, DiscImageFormat targetFormat, IInjectionWorkspace workspace, CancellationToken cancellationToken);
    }
}
