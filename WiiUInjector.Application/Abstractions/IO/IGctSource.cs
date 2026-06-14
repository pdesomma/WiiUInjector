using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Abstractions.IO
{
    /// <summary>
    /// Abstracts reading GCT (Game Configuration Table) files in binary or text format.
    /// </summary>
    public interface IGctSource
    {
        /// <summary>
        /// Reads a GCT file as binary bytes asynchronously.
        /// </summary>
        /// <param name="filePath">Absolute or relative path to the GCT file.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the raw bytes on success, or validation errors on failure.</returns>
        Task<Result<byte[]>> ReadBytesAsync(string filePath, CancellationToken cancellationToken);

        /// <summary>
        /// Reads a GCT file as text asynchronously.
        /// </summary>
        /// <param name="filePath">Absolute or relative path to the GCT file.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the file text on success, or validation errors on failure.</returns>
        Task<Result<string>> ReadTextAsync(string filePath, CancellationToken cancellationToken);

        /// <summary>
        /// Determines whether a GCT file is in binary format (as opposed to text/code format).
        /// Implementation may decide based on file extension or content sniffing.
        /// </summary>
        /// <param name="filePath">Absolute or relative path to the GCT file.</param>
        /// <returns>true if the file is binary format; false if text/code format.</returns>
        bool IsBinaryGctFile(string filePath);
    }
}
