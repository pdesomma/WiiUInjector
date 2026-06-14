using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Services.Injections
{
    /// <summary>
    /// Service interface for orchestrating ROM injection operations: building injection specifications,
    /// validating them, and executing the injection pipeline.
    /// </summary>
    public interface IInjectionService
    {
        /// <summary>
        /// Builds an Injection aggregate by materializing value objects from loaders (ROM, images, boot sound)
        /// and resolving base ROM from the catalog. Runs the domain validator to catch business rule violations.
        /// </summary>
        /// <param name="command">The build command with user inputs; must not be null.</param>
        /// <param name="cancellationToken">Cancellation token for async IO operations (ROM/image/sound loading, catalog lookup).</param>
        /// <returns>
        /// A Result containing the constructed Injection on success. On failure, returns Result&lt;Injection&gt;.Failure
        /// with application-layer errors mapped from the domain validator and loaders.
        /// </returns>
        Task<Result<Injection>> BuildAsync(BuildInjectionCommand command, CancellationToken cancellationToken);

        /// <summary>
        /// Synchronously validates an Injection against domain business rules.
        /// Useful for live UI feedback when options are changed without rebuilding the aggregate.
        /// </summary>
        /// <param name="injection">The injection to validate; must not be null.</param>
        /// <returns>
        /// Result.Success() if validation passes. On failure, returns Result.Failure with application-layer errors
        /// mapped from the domain validator.
        /// </returns>
        Result Validate(Injection injection);

        /// <summary>
        /// Executes the complete ROM injection pipeline: verifies all required external tools are present,
        /// re-validates the injection, and orchestrates the pipeline to produce a packaged output.
        /// Emits progress events and respects cancellation.
        /// </summary>
        /// <param name="command">The run command with injection spec and output config; must not be null.</param>
        /// <param name="progress">Progress reporter for pipeline events (e.g., stage completion, file creation); must not be null.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>
        /// A Result containing the InjectionOutcome on success (path to output package, etc.).
        /// On failure, returns Result&lt;InjectionOutcome&gt;.Failure with application-layer errors
        /// (missing tools, validation failures, pipeline errors, user cancellation).
        /// </returns>
        Task<Result<InjectionOutcome>> RunAsync(
            RunInjectionCommand command,
            IProgress<InjectionProgress> progress,
            CancellationToken cancellationToken);
    }
}
