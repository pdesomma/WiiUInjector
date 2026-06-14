using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Services.Injections
{
    /// <summary>
    /// Interface for executing the complete ROM injection pipeline for a given Injection specification.
    /// </summary>
    public interface IInjectionPipeline
    {
        /// <summary>
        /// Executes the injection pipeline asynchronously for the given injection specification and output configuration.
        /// </summary>
        /// <param name="injection">The injection specification containing ROM, console type, images, and emulator options.</param>
        /// <param name="output">The output configuration specifying directory, format, and key source.</param>
        /// <param name="progress">An IProgress instance for reporting injection progress; may be null.</param>
        /// <param name="cancellationToken">A token for cancelling the operation.</param>
        /// <returns>
        /// A Result containing an InjectionOutcome if successful, or an error result if the injection fails.
        /// </returns>
        Task<Result<InjectionOutcome>> RunAsync(
            Injection injection,
            InjectionOutputSpec output,
            IProgress<InjectionProgress> progress,
            CancellationToken cancellationToken);
    }
}
