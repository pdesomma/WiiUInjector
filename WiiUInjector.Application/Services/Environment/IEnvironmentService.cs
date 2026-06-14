using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Environment;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Services.Environment
{
    /// <summary>
    /// Service for inspecting system environment conditions.
    /// </summary>
    public interface IEnvironmentService
    {
        /// <summary>
        /// Inspects the current system environment and returns a report.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the EnvironmentReport on success, or errors on failure.</returns>
        Task<Result<EnvironmentReport>> InspectAsync(CancellationToken cancellationToken);
    }
}
