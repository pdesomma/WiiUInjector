using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Abstractions.Operations
{
    /// <summary>
    /// Abstraction for editing Wii U package metadata files.
    /// Patches meta.xml and app.xml with the provided metadata (Title ID, product code, group ID, boot screen data, DRC toggle).
    /// </summary>
    public interface IWiiUMetadataEditor
    {
        /// <summary>
        /// Edits the Wii U metadata files in the workspace with the specified metadata.
        /// </summary>
        /// <param name="metadata">The metadata to write into the workspace metadata files.</param>
        /// <param name="workspace">The injection workspace containing the metadata files.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A Result indicating success or containing errors if editing failed.</returns>
        Task<Result> EditAsync(WiiUMetadata metadata, IInjectionWorkspace workspace, CancellationToken cancellationToken);
    }
}
