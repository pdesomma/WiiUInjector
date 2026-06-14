using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Logging;
using WiiUInjector.Application.Abstractions.Operations;
using WiiUInjector.Application.Abstractions.Operations.Injectors;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Services.Injections
{
    /// <summary>
    /// Sealed implementation of IInjectionPipeline. Orchestrates the complete ROM injection workflow
    /// by coordinating workspace management, base ROM extraction, per-console ROM injection, metadata editing,
    /// boot sound encoding, and final packaging.
    /// </summary>
    public sealed class InjectionPipeline : IInjectionPipeline
    {
        private readonly IInjectionWorkspaceFactory _workspaceFactory;
        private readonly IBaseRomExtractor _baseRomExtractor;
        private readonly INesSnesRomInjector _nesSnesInjector;
        private readonly IN64RomInjector _n64Injector;
        private readonly IGbaRomInjector _gbaInjector;
        private readonly INdsRomInjector _ndsInjector;
        private readonly ITurboGrafxRomInjector _turboGrafxInjector;
        private readonly IMsxRomInjector _msxInjector;
        private readonly IWiiIsoInjector _wiiInjector;
        private readonly IGcnIsoInjector _gcnInjector;
        private readonly IWiiUMetadataEditor _metadataEditor;
        private readonly IWiiUBootSoundWriter _bootSoundWriter;
        private readonly IWiiUPackager _packager;
        private readonly IApplicationLogger _logger;

        /// <summary>
        /// Initializes a new InjectionPipeline with all required operation dependencies.
        /// </summary>
        /// <param name="workspaceFactory">Factory for creating injection workspaces.</param>
        /// <param name="baseRomExtractor">Extracts base ROM packages into the workspace.</param>
        /// <param name="nesSnesInjector">Injects NES and SNES ROMs.</param>
        /// <param name="n64Injector">Injects Nintendo 64 ROMs.</param>
        /// <param name="gbaInjector">Injects Game Boy Advance ROMs.</param>
        /// <param name="ndsInjector">Injects Nintendo DS ROMs.</param>
        /// <param name="turboGrafxInjector">Injects TurboGrafx-16 ROMs.</param>
        /// <param name="msxInjector">Injects MSX ROMs.</param>
        /// <param name="wiiInjector">Injects Wii ISOs.</param>
        /// <param name="gcnInjector">Injects GameCube ISOs.</param>
        /// <param name="metadataEditor">Edits Wii U package metadata files.</param>
        /// <param name="bootSoundWriter">Writes boot sound data to the package.</param>
        /// <param name="packager">Packages the final Wii U artifact.</param>
        /// <param name="logger">Application-wide logger.</param>
        /// <exception cref="ArgumentNullException">Thrown if any dependency is null.</exception>
        public InjectionPipeline(
            IInjectionWorkspaceFactory workspaceFactory,
            IBaseRomExtractor baseRomExtractor,
            INesSnesRomInjector nesSnesInjector,
            IN64RomInjector n64Injector,
            IGbaRomInjector gbaInjector,
            INdsRomInjector ndsInjector,
            ITurboGrafxRomInjector turboGrafxInjector,
            IMsxRomInjector msxInjector,
            IWiiIsoInjector wiiInjector,
            IGcnIsoInjector gcnInjector,
            IWiiUMetadataEditor metadataEditor,
            IWiiUBootSoundWriter bootSoundWriter,
            IWiiUPackager packager,
            IApplicationLogger logger)
        {
            _workspaceFactory = workspaceFactory ?? throw new ArgumentNullException(nameof(workspaceFactory));
            _baseRomExtractor = baseRomExtractor ?? throw new ArgumentNullException(nameof(baseRomExtractor));
            _nesSnesInjector = nesSnesInjector ?? throw new ArgumentNullException(nameof(nesSnesInjector));
            _n64Injector = n64Injector ?? throw new ArgumentNullException(nameof(n64Injector));
            _gbaInjector = gbaInjector ?? throw new ArgumentNullException(nameof(gbaInjector));
            _ndsInjector = ndsInjector ?? throw new ArgumentNullException(nameof(ndsInjector));
            _turboGrafxInjector = turboGrafxInjector ?? throw new ArgumentNullException(nameof(turboGrafxInjector));
            _msxInjector = msxInjector ?? throw new ArgumentNullException(nameof(msxInjector));
            _wiiInjector = wiiInjector ?? throw new ArgumentNullException(nameof(wiiInjector));
            _gcnInjector = gcnInjector ?? throw new ArgumentNullException(nameof(gcnInjector));
            _metadataEditor = metadataEditor ?? throw new ArgumentNullException(nameof(metadataEditor));
            _bootSoundWriter = bootSoundWriter ?? throw new ArgumentNullException(nameof(bootSoundWriter));
            _packager = packager ?? throw new ArgumentNullException(nameof(packager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Executes the complete ROM injection pipeline asynchronously.
        /// Orchestrates workspace creation, base ROM extraction, ROM injection, metadata editing,
        /// boot sound writing, and final packaging.
        /// </summary>
        /// <param name="injection">The injection aggregate containing console type, ROM, images, and options.</param>
        /// <param name="output">The output configuration specifying destination directory and format.</param>
        /// <param name="progress">Progress reporter for UI updates; receives stage milestones and percentage.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A Result containing the InjectionOutcome on success, or application errors on failure.</returns>
        public async Task<Result<InjectionOutcome>> RunAsync(
            Injection injection,
            InjectionOutputSpec output,
            IProgress<InjectionProgress> progress,
            CancellationToken cancellationToken)
        {
            // Null-check inputs.
            if (injection == null)
                return Result<InjectionOutcome>.Failure(
                    new ApplicationError(ApplicationErrors.PipelineFailed, "Injection cannot be null.", nameof(injection)));
            if (output == null)
                return Result<InjectionOutcome>.Failure(
                    new ApplicationError(ApplicationErrors.PipelineFailed, "Output spec cannot be null.", nameof(output)));
            if (progress == null)
                return Result<InjectionOutcome>.Failure(
                    new ApplicationError(ApplicationErrors.PipelineFailed, "Progress reporter cannot be null.", nameof(progress)));

            try
            {
                // Log injection start.
                _logger.Info($"Starting injection for {injection.Console} → {injection.RomFileName}");

                // Acquire workspace.
                var workspaceResult = await _workspaceFactory.CreateAsync(injection.Console, cancellationToken);
                if (workspaceResult.IsFailure)
                    return MapFailure<InjectionOutcome>(workspaceResult, ApplicationErrors.WorkspaceCreateFailed);

                using (var workspace = workspaceResult.Value)
                {
                    // Report progress: extracting base ROM.
                    progress.Report(new InjectionProgress(5, "Extracting base ROM", null));

                    var extractResult = await _baseRomExtractor.ExtractAsync(injection.BaseRom, workspace, cancellationToken);
                    if (extractResult.IsFailure)
                        return MapFailure<InjectionOutcome>(extractResult, ApplicationErrors.BaseRomExtractFailed);

                    // Report progress: injecting ROM.
                    progress.Report(new InjectionProgress(15, "Injecting ROM", null));

                    // Dispatch to per-console injector via switch expression.
                    var injectTask = injection.Console switch
                    {
                        ConsoleType.Nes => _nesSnesInjector.InjectAsync(injection, workspace, cancellationToken),
                        ConsoleType.Snes => _nesSnesInjector.InjectAsync(injection, workspace, cancellationToken),
                        ConsoleType.N64 => _n64Injector.InjectAsync(injection, workspace, cancellationToken),
                        ConsoleType.Gba => _gbaInjector.InjectAsync(injection, workspace, cancellationToken),
                        ConsoleType.Nds => _ndsInjector.InjectAsync(injection, workspace, cancellationToken),
                        ConsoleType.TurboGrafx => _turboGrafxInjector.InjectAsync(injection, workspace, cancellationToken),
                        ConsoleType.Msx => _msxInjector.InjectAsync(injection, workspace, cancellationToken),
                        ConsoleType.Wii => _wiiInjector.InjectAsync(injection, workspace, cancellationToken),
                        ConsoleType.Gcn => _gcnInjector.InjectAsync(injection, workspace, cancellationToken),
                        // Defensive: enum is closed, should never reach this.
                        _ => throw new InvalidOperationException($"No injector for console type {injection.Console}.")
                    };

                    var injectResult = await injectTask;
                    if (injectResult.IsFailure)
                        return MapFailure<InjectionOutcome>(injectResult, ApplicationErrors.RomInjectionFailed);

                    // Report progress: editing metadata.
                    progress.Report(new InjectionProgress(80, "Editing metadata", null));

                    // Build WiiUMetadata from injection. Placeholder fields will be derived by infra impl.
                    WiiUMetadata metadata;
                    try
                    {
                        // Placeholder: derive ProductCode from TitleId. Infra impl can override this logic.
                        string productCode = injection.BaseRom.TitleId.Length >= 8
                            ? injection.BaseRom.TitleId.Substring(8)
                            : "WUP-N-XXXX";

                        // Placeholder: GroupId is always "0000" for now. Infra impl can derive properly.
                        metadata = new WiiUMetadata(
                            injection.BaseRom.TitleId,
                            productCode,
                            "0000",
                            injection.BootScreen,
                            true); // Placeholder: UseDrcScreen = true for now.
                    }
                    catch (ArgumentException ex)
                    {
                        return Result<InjectionOutcome>.Failure(
                            new ApplicationError(ApplicationErrors.MetadataEditFailed, ex.Message, null));
                    }

                    var editResult = await _metadataEditor.EditAsync(metadata, workspace, cancellationToken);
                    if (editResult.IsFailure)
                        return MapFailure<InjectionOutcome>(editResult, ApplicationErrors.MetadataEditFailed);

                    // Write boot sound if present.
                    if (injection.BootSound != null)
                    {
                        progress.Report(new InjectionProgress(90, "Writing boot sound", null));

                        var writeResult = await _bootSoundWriter.WriteAsync(injection.BootSound, workspace, cancellationToken);
                        if (writeResult.IsFailure)
                            return MapFailure<InjectionOutcome>(writeResult, ApplicationErrors.BootSoundWriteFailed);
                    }

                    // Report progress: packaging output.
                    progress.Report(new InjectionProgress(95, "Packaging output", null));

                    var packResult = await _packager.PackAsync(workspace, output, cancellationToken);
                    if (packResult.IsFailure)
                        return MapFailure<InjectionOutcome>(packResult, ApplicationErrors.PackagingFailed);

                    // Log completion.
                    _logger.Info($"Injection complete: {packResult.Value.OutputPath}");

                    // Report final progress.
                    progress.Report(new InjectionProgress(100, "Done", null));

                    return packResult;
                }
            }
            catch (OperationCanceledException)
            {
                _logger.Error("Injection cancelled by user.");
                return Result<InjectionOutcome>.Failure(
                    new ApplicationError(ApplicationErrors.OperationCancelled, "Injection cancelled by user.", null));
            }
        }

        /// <summary>
        /// Re-tags the first error from a failed result with a pipeline-stage-specific error code,
        /// preserving the original error message and field.
        /// </summary>
        /// <typeparam name="T">The result value type.</typeparam>
        /// <param name="source">The source Result to re-tag.</param>
        /// <param name="errorCode">The pipeline stage error code to apply (e.g., BaseRomExtractFailed).</param>
        /// <returns>A failure Result with the first error re-tagged with the stage code.</returns>
        private static Result<T> MapFailure<T>(Result source, string errorCode)
        {
            // Re-tag the first error with the pipeline stage's well-known code, preserving its message.
            var first = source.Errors.Count > 0 ? source.Errors[0] : null;
            var message = first != null ? first.Message : "Pipeline step failed.";
            var field = first != null ? first.Field : null;
            var tagged = new ApplicationError(errorCode, message, field);
            return Result<T>.Failure(tagged);
        }
    }
}
