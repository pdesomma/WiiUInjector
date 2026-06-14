using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Abstractions.Operations
{
    /// <summary>
    /// Abstraction for encoding ISO disc images into NFS (Nintendo File System) format.
    /// Used for Wii and GameCube ROM injections.
    /// </summary>
    public interface INfsEncoder
    {
        /// <summary>
        /// Encodes an ISO disc image into NFS format with the specified options.
        /// The encoded output is written to the workspace content directory.
        /// </summary>
        /// <param name="isoPath">Absolute path to the source ISO file.</param>
        /// <param name="options">Encoding options (flags, encryption key, etc.).</param>
        /// <param name="workspace">The injection workspace where the NFS output will be written.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A Result indicating success or containing errors if encoding failed.</returns>
        Task<Result> EncodeAsync(string isoPath, NfsEncodingOptions options, IInjectionWorkspace workspace, CancellationToken cancellationToken);
    }
}
