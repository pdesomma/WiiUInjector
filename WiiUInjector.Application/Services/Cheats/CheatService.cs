using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.IO;
using WiiUInjector.Application.Abstractions.Logging;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;
using WiiUInjector.Domain.Services;

namespace WiiUInjector.Application.Services.Cheats
{
    /// <summary>
    /// Service implementation for loading GCT cheat codes, patching DOL executables, and injecting codehandlers.
    /// </summary>
    public sealed class CheatService : ICheatService
    {
        private readonly IGctSource _source;
        private readonly IGctParser _parser;
        private readonly IDolPatcher _patcher;
        private readonly IApplicationLogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheatService"/> class.
        /// </summary>
        /// <param name="source">Service for reading GCT files.</param>
        /// <param name="parser">Service for parsing GCT code lists.</param>
        /// <param name="patcher">Service for patching DOL executables.</param>
        /// <param name="logger">Service for logging.</param>
        /// <exception cref="ArgumentNullException">Thrown when any dependency is null.</exception>
        public CheatService(IGctSource source, IGctParser parser, IDolPatcher patcher, IApplicationLogger logger)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (parser == null)
                throw new ArgumentNullException(nameof(parser));

            if (patcher == null)
                throw new ArgumentNullException(nameof(patcher));

            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _source = source;
            _parser = parser;
            _patcher = patcher;
            _logger = logger;
        }

        /// <summary>
        /// Loads GCT cheat codes from a file, auto-detecting binary vs text format.
        /// </summary>
        /// <param name="filePath">Path to the GCT file (binary .gct or text codelist).</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the loaded codes on success, or an error on failure.</returns>
        public async Task<Result<IReadOnlyList<GctCode>>> LoadCodesAsync(string filePath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                var error = new ApplicationError(ApplicationErrors.GctParseFailed, "filePath cannot be null or empty.", nameof(filePath));
                _logger.Error(error.Message);
                return Result<IReadOnlyList<GctCode>>.Failure(error);
            }

            try
            {
                // Auto-detect binary vs text format.
                bool isBinary = _source.IsBinaryGctFile(filePath);

                IReadOnlyList<GctCode> codes;

                if (isBinary)
                {
                    // Binary .gct file: read bytes then parse.
                    var bytesResult = await _source.ReadBytesAsync(filePath, cancellationToken);
                    if (bytesResult.IsFailure)
                        return Result<IReadOnlyList<GctCode>>.Failure(bytesResult);

                    codes = _parser.ParseBinary(bytesResult.Value);
                }
                else
                {
                    // Text codelist: read text then parse.
                    var textResult = await _source.ReadTextAsync(filePath, cancellationToken);
                    if (textResult.IsFailure)
                        return Result<IReadOnlyList<GctCode>>.Failure(textResult);

                    codes = _parser.ParseText(textResult.Value);
                }

                _logger.Info($"Successfully loaded {codes.Count} cheat codes from '{filePath}'.");
                return Result<IReadOnlyList<GctCode>>.Success(codes);
            }
            catch (Exception ex)
            {
                var error = new ApplicationError(ApplicationErrors.GctParseFailed, ex.Message);
                _logger.Error($"Failed to load cheat codes from '{filePath}': {ex.Message}", ex);
                return Result<IReadOnlyList<GctCode>>.Failure(error);
            }
        }

        /// <summary>
        /// Applies a list of GCT cheat codes to a DOL executable buffer.
        /// </summary>
        /// <param name="dolBytes">Raw DOL executable bytes.</param>
        /// <param name="codes">Cheat codes to apply.</param>
        /// <returns>A Result containing the patched DOL on success, or an error on failure.</returns>
        public Result<byte[]> PatchDol(byte[] dolBytes, IReadOnlyList<GctCode> codes)
        {
            if (dolBytes == null)
            {
                var error = new ApplicationError(ApplicationErrors.DolPatchFailed, "dolBytes cannot be null.", nameof(dolBytes));
                _logger.Error(error.Message);
                return Result<byte[]>.Failure(error);
            }

            if (codes == null)
            {
                var error = new ApplicationError(ApplicationErrors.DolPatchFailed, "codes cannot be null.", nameof(codes));
                _logger.Error(error.Message);
                return Result<byte[]>.Failure(error);
            }

            try
            {
                // If no codes to apply, return the DOL unchanged (defensive no-op).
                if (codes.Count == 0)
                {
                    _logger.Info("No cheat codes to apply; returning DOL unchanged.");
                    return Result<byte[]>.Success(dolBytes);
                }

                byte[] patchedDol = _patcher.ApplyCodes(dolBytes, codes);
                _logger.Info($"Successfully applied {codes.Count} cheat codes to DOL.");
                return Result<byte[]>.Success(patchedDol);
            }
            catch (Exception ex)
            {
                var error = new ApplicationError(ApplicationErrors.DolPatchFailed, ex.Message);
                _logger.Error($"Failed to apply cheat codes to DOL: {ex.Message}", ex);
                return Result<byte[]>.Failure(error);
            }
        }

        /// <summary>
        /// Injects a codehandler into a DOL executable and updates the entry point.
        /// </summary>
        /// <param name="dolBytes">Raw DOL executable bytes.</param>
        /// <param name="codehandler">Codehandler binary to inject.</param>
        /// <returns>A Result containing the modified DOL on success, or an error on failure.</returns>
        public Result<byte[]> InjectCodehandler(byte[] dolBytes, byte[] codehandler)
        {
            if (dolBytes == null)
            {
                var error = new ApplicationError(ApplicationErrors.DolPatchFailed, "dolBytes cannot be null.", nameof(dolBytes));
                _logger.Error(error.Message);
                return Result<byte[]>.Failure(error);
            }

            if (codehandler == null)
            {
                var error = new ApplicationError(ApplicationErrors.DolPatchFailed, "codehandler cannot be null.", nameof(codehandler));
                _logger.Error(error.Message);
                return Result<byte[]>.Failure(error);
            }

            try
            {
                byte[] injectedDol = _patcher.InjectCodehandler(dolBytes, codehandler);
                _logger.Info("Successfully injected codehandler into DOL.");
                return Result<byte[]>.Success(injectedDol);
            }
            catch (Exception ex)
            {
                var error = new ApplicationError(ApplicationErrors.DolPatchFailed, ex.Message);
                _logger.Error($"Failed to inject codehandler into DOL: {ex.Message}", ex);
                return Result<byte[]>.Failure(error);
            }
        }
    }
}
