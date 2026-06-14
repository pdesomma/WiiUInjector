using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Abstractions.Operations
{
    /// <summary>
    /// Factory abstraction for creating injection workspaces.
    /// Encapsulates the decision of where workspaces are created (e.g., %TEMP% or bin\temp\).
    /// </summary>
    public interface IInjectionWorkspaceFactory
    {
        /// <summary>
        /// Creates a new injection workspace for the specified console type.
        /// The factory determines the temp root location and ensures all required subdirectories are initialized.
        /// </summary>
        /// <param name="console">The target console type (used to organize workspace paths).</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A Result containing the created workspace on success, or errors on failure.</returns>
        Task<Result<IInjectionWorkspace>> CreateAsync(ConsoleType console, CancellationToken cancellationToken);
    }
}
