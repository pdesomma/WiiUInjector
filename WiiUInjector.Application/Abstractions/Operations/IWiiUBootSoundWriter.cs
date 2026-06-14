using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Abstractions.Operations
{
    /// <summary>
    /// Abstraction for writing boot sound data to Wii U package metadata.
    /// Encodes audio from the provided BootSound (clipping to 6 seconds, downmixing to 48kHz stereo if needed)
    /// and writes the result to meta\bootSound.btsnd inside the workspace.
    /// </summary>
    public interface IWiiUBootSoundWriter
    {
        /// <summary>
        /// Writes the boot sound to the Wii U metadata directory in the workspace.
        /// </summary>
        /// <param name="bootSound">The boot sound to encode and write.</param>
        /// <param name="workspace">The injection workspace where the boot sound file will be written.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A Result indicating success or containing errors if writing failed.</returns>
        Task<Result> WriteAsync(BootSound bootSound, IInjectionWorkspace workspace, CancellationToken cancellationToken);
    }
}
