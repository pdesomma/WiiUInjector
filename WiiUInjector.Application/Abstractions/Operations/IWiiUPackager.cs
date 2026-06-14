using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.Injections;

namespace WiiUInjector.Application.Abstractions.Operations
{
    /// <summary>
    /// Abstraction for packaging the contents of an injection workspace into the final Wii U artifact.
    /// Produces either an encrypted WUP (packaged) or Loadiine (directory) format artifact
    /// based on the output specification.
    /// </summary>
    public interface IWiiUPackager
    {
        /// <summary>
        /// Packages the workspace contents into the final Wii U artifact.
        /// </summary>
        /// <param name="workspace">The fully-prepared injection workspace (with base ROM, content, metadata, etc.).</param>
        /// <param name="output">The output specification (format, destination directory, encryption key if needed).</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A Result containing the InjectionOutcome with the final artifact path and metadata on success, or errors on failure.</returns>
        Task<Result<InjectionOutcome>> PackAsync(IInjectionWorkspace workspace, InjectionOutputSpec output, CancellationToken cancellationToken);
    }
}
