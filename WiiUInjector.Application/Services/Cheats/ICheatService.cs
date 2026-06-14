using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Services.Cheats
{
    /// <summary>
    /// Service for loading GCT cheat codes, patching DOL executables, and injecting codehandlers.
    /// </summary>
    public interface ICheatService
    {
        /// <summary>
        /// Loads GCT cheat codes from a file, auto-detecting binary vs text format.
        /// Auto-detects via IGctSource.IsBinaryGctFile, then dispatches to IGctParser.ParseBinary or ParseText accordingly.
        /// </summary>
        /// <param name="filePath">Path to the GCT file (binary .gct or text codelist).</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result containing the loaded codes on success, or an error on failure.</returns>
        Task<Result<IReadOnlyList<GctCode>>> LoadCodesAsync(string filePath, CancellationToken cancellationToken);

        /// <summary>
        /// Applies a list of GCT cheat codes to a DOL executable buffer.
        /// Wraps IDolPatcher.ApplyCodes to translate memory addresses and overwrite values.
        /// </summary>
        /// <param name="dolBytes">Raw DOL executable bytes.</param>
        /// <param name="codes">Cheat codes to apply.</param>
        /// <returns>A Result containing the patched DOL on success, or an error on failure.</returns>
        Result<byte[]> PatchDol(byte[] dolBytes, IReadOnlyList<GctCode> codes);

        /// <summary>
        /// Injects a codehandler into a DOL executable and updates the entry point.
        /// Wraps IDolPatcher.InjectCodehandler to find free space and wire the entry point.
        /// </summary>
        /// <param name="dolBytes">Raw DOL executable bytes.</param>
        /// <param name="codehandler">Codehandler binary to inject.</param>
        /// <returns>A Result containing the modified DOL on success, or an error on failure.</returns>
        Result<byte[]> InjectCodehandler(byte[] dolBytes, byte[] codehandler);
    }
}
