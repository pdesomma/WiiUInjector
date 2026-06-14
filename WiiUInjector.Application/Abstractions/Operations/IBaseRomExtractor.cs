using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Abstractions.Operations
{
    /// <summary>
    /// Abstraction for extracting base ROM packages into a workspace.
    /// For curated (packaged) bases, extracts the BASE.zip or equivalent.
    /// For custom user bases, copies from the user-supplied source path.
    /// Details of the extraction mechanism are owned by the infrastructure implementation.
    /// </summary>
    public interface IBaseRomExtractor
    {
        /// <summary>
        /// Extracts the base ROM package into the workspace.
        /// </summary>
        /// <param name="baseRom">The base ROM metadata and source.</param>
        /// <param name="workspace">The injection workspace where the base ROM will be extracted.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A Result indicating success or containing errors if extraction failed.</returns>
        Task<Result> ExtractAsync(BaseRom baseRom, IInjectionWorkspace workspace, CancellationToken cancellationToken);
    }
}
