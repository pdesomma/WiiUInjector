using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Abstractions.Environment
{
    /// <summary>
    /// Interface for inspecting system environment conditions.
    /// </summary>
    public interface IEnvironmentInspector
    {
        /// <summary>
        /// Inspects the system environment asynchronously and returns a report.
        /// </summary>
        /// <param name="cancellationToken">A token for cancelling the operation.</param>
        /// <returns>
        /// A Result containing an EnvironmentReport if the inspection succeeds, or an error result if it fails.
        /// </returns>
        Task<Result<EnvironmentReport>> InspectAsync(CancellationToken cancellationToken);
    }
}
