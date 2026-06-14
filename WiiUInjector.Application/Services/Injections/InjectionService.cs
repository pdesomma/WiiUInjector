using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Catalog;
using WiiUInjector.Application.Abstractions.IO;
using WiiUInjector.Application.Abstractions.Logging;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;
using WiiUInjector.Domain.EmulatorOptions;
using WiiUInjector.Domain.Services;

namespace WiiUInjector.Application.Services.Injections
{
    /// <summary>
    /// Sealed implementation of IInjectionService. Orchestrates ROM injection: building aggregates via loaders,
    /// running domain validation, provisioning external tools, and delegating pipeline execution.
    /// </summary>
    public sealed class InjectionService : IInjectionService
    {
        private readonly IRomFileLoader _romFileLoader;
        private readonly IGameImageLoader _imageLoader;
        private readonly IBootSoundLoader _soundLoader;
        private readonly IBaseRomCatalog _baseRomCatalog;
        private readonly IInjectionValidator _validator;
        private readonly IInjectionPipeline _pipeline;
        private readonly IApplicationLogger _logger;

        /// <summary>
        /// Initializes a new InjectionService with all required dependencies.
        /// </summary>
        /// <param name="romFileLoader">Loads ROM files and returns IRomSource instances.</param>
        /// <param name="imageLoader">Loads image files and materializes GameImage value objects.</param>
        /// <param name="soundLoader">Loads boot sound files and materializes BootSound value objects.</param>
        /// <param name="baseRomCatalog">Provides access to curated and custom base ROM entries.</param>
        /// <param name="validator">Domain validator for Injection business rules.</param>
        /// <param name="pipeline">Executes the ROM injection pipeline.</param>
        /// <param name="logger">Application-wide logger.</param>
        /// <exception cref="ArgumentNullException">Thrown if any dependency is null.</exception>
        public InjectionService(
            IRomFileLoader romFileLoader,
            IGameImageLoader imageLoader,
            IBootSoundLoader soundLoader,
            IBaseRomCatalog baseRomCatalog,
            IInjectionValidator validator,
            IInjectionPipeline pipeline,
            IApplicationLogger logger)
        {
            _romFileLoader = romFileLoader ?? throw new ArgumentNullException(nameof(romFileLoader));
            _imageLoader = imageLoader ?? throw new ArgumentNullException(nameof(imageLoader));
            _soundLoader = soundLoader ?? throw new ArgumentNullException(nameof(soundLoader));
            _baseRomCatalog = baseRomCatalog ?? throw new ArgumentNullException(nameof(baseRomCatalog));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Builds an Injection aggregate by materializing value objects from loaders and resolving base ROM.
        /// Runs domain validation to enforce business rules.
        /// </summary>
        public async Task<Result<Injection>> BuildAsync(BuildInjectionCommand command, CancellationToken cancellationToken)
        {
            // Null-check command.
            if (command == null)
                return Result<Injection>.Failure(
                    new ApplicationError(ApplicationErrors.DomainInvariantViolation, "Build command cannot be null.", nameof(command)));

            // Resolve base ROM from catalog or custom file.
            Result<BaseRom> baseRomResult = command.BaseRomSelection.Kind switch
            {
                BaseRomSelectionKind.Curated => await _baseRomCatalog.FindCuratedAsync(
                    command.Console,
                    command.BaseRomSelection.TitleId,
                    command.BaseRomSelection.Region,
                    cancellationToken),

                BaseRomSelectionKind.Custom => await _baseRomCatalog.LoadCustomAsync(
                    command.Console,
                    command.BaseRomSelection.CustomPath,
                    cancellationToken),

                _ => Result<BaseRom>.Failure(
                    new ApplicationError(ApplicationErrors.DomainInvariantViolation, $"Unknown base ROM selection kind: {command.BaseRomSelection.Kind}.", null))
            };

            if (baseRomResult.IsFailure)
                return Result<Injection>.Failure(baseRomResult.Errors);

            BaseRom baseRom = baseRomResult.Value;

            // Load ROM file.
            var romLoadResult = await _romFileLoader.LoadAsync(command.RomPath, cancellationToken);
            if (romLoadResult.IsFailure)
                return Result<Injection>.Failure(romLoadResult.Errors);

            IRomSource rom = romLoadResult.Value.Rom;
            string romFileName = romLoadResult.Value.FileName;

            // Load all four images concurrently.
            var imageTasks = new Dictionary<ImageSlot, Task<Result<GameImage>>>
            {
                { ImageSlot.Icon, _imageLoader.LoadAsync(command.ImagePaths[ImageSlot.Icon], ImageSlot.Icon, cancellationToken) },
                { ImageSlot.Drc, _imageLoader.LoadAsync(command.ImagePaths[ImageSlot.Drc], ImageSlot.Drc, cancellationToken) },
                { ImageSlot.Tv, _imageLoader.LoadAsync(command.ImagePaths[ImageSlot.Tv], ImageSlot.Tv, cancellationToken) },
                { ImageSlot.Logo, _imageLoader.LoadAsync(command.ImagePaths[ImageSlot.Logo], ImageSlot.Logo, cancellationToken) }
            };

            await Task.WhenAll(imageTasks.Values);

            // Aggregate any image load failures; if any failed, return all errors.
            var imageErrors = new List<ApplicationError>();
            var images = new Dictionary<ImageSlot, GameImage>();

            foreach (var kvp in imageTasks)
            {
                if (kvp.Value.Result.IsFailure)
                    imageErrors.AddRange(kvp.Value.Result.Errors);
                else
                    images[kvp.Key] = kvp.Value.Result.Value;
            }

            if (imageErrors.Count > 0)
                return Result<Injection>.Failure(imageErrors);

            // Load boot sound if provided.
            BootSound bootSound = null;
            if (!string.IsNullOrWhiteSpace(command.BootSoundPath))
            {
                var soundLoadResult = await _soundLoader.LoadAsync(command.BootSoundPath, cancellationToken);
                if (soundLoadResult.IsFailure)
                    return Result<Injection>.Failure(soundLoadResult.Errors);

                bootSound = soundLoadResult.Value;
            }

            // Resolve emulator options: use provided or defaults.
            IEmulatorOptions options = command.Options ?? EmulatorOptionsDefaults.For(command.Console);

            // Verify options target the correct console.
            if (options.Console != command.Console)
            {
                return Result<Injection>.Failure(
                    new ApplicationError(
                        ApplicationErrors.EmulatorOptionsConsoleMismatch,
                        $"Options target {options.Console} but command targets {command.Console}.",
                        nameof(command.Options)));
            }

            // Construct the Injection aggregate; wrap domain invariant violations.
            Injection injection;
            try
            {
                injection = new Injection(
                    command.Console,
                    baseRom,
                    rom,
                    romFileName,
                    images,
                    command.BootScreen,
                    options,
                    bootSound);
            }
            catch (ArgumentNullException ex)
            {
                return Result<Injection>.Failure(
                    new ApplicationError(ApplicationErrors.DomainInvariantViolation, ex.Message, ex.ParamName));
            }
            catch (ArgumentException ex)
            {
                return Result<Injection>.Failure(
                    new ApplicationError(ApplicationErrors.DomainInvariantViolation, ex.Message, ex.ParamName));
            }

            // Run domain validator.
            ValidationResult validationResult = _validator.Validate(injection);
            if (!validationResult.IsValid)
                return DomainErrorMapper.ToResult<Injection>(validationResult, injection);

            return Result<Injection>.Success(injection);
        }

        /// <summary>
        /// Synchronously validates an Injection against domain business rules.
        /// </summary>
        public Result Validate(Injection injection)
        {
            // Null-check injection.
            if (injection == null)
                return Result.Failure(
                    new ApplicationError(ApplicationErrors.DomainInvariantViolation, "Injection cannot be null.", nameof(injection)));

            // Run domain validator.
            ValidationResult validationResult = _validator.Validate(injection);

            // Map domain errors to application errors.
            return DomainErrorMapper.ToResult(validationResult);
        }

        /// <summary>
        /// Executes the ROM injection pipeline: checks tools, re-validates, and delegates to pipeline.
        /// </summary>
        public async Task<Result<InjectionOutcome>> RunAsync(
            RunInjectionCommand command,
            IProgress<InjectionProgress> progress,
            CancellationToken cancellationToken)
        {
            // Null-check command.
            if (command == null)
                return Result<InjectionOutcome>.Failure(
                    new ApplicationError(ApplicationErrors.DomainInvariantViolation, "Run command cannot be null.", nameof(command)));

            // Null-check progress.
            if (progress == null)
                return Result<InjectionOutcome>.Failure(
                    new ApplicationError(ApplicationErrors.DomainInvariantViolation, "Progress reporter cannot be null.", nameof(progress)));

            // Re-validate the injection.
            var validateResult = Validate(command.Injection);
            if (validateResult.IsFailure)
                return Result<InjectionOutcome>.Failure(validateResult.Errors);

            // Log injection start.
            _logger.Info($"Starting injection for {command.Injection.Console} {command.Injection.RomFileName}");

            // Run pipeline.
            try
            {
                var pipelineResult = await _pipeline.RunAsync(
                    command.Injection,
                    command.Output,
                    progress,
                    cancellationToken);

                return pipelineResult;
            }
            catch (OperationCanceledException)
            {
                return Result<InjectionOutcome>.Failure(
                    new ApplicationError(ApplicationErrors.OperationCancelled, "Injection cancelled by user.", null));
            }
            catch (Exception ex)
            {
                _logger.Error("Injection pipeline failed.", ex);
                return Result<InjectionOutcome>.Failure(
                    new ApplicationError(ApplicationErrors.PipelineFailed, ex.Message, null));
            }
        }
    }
}
