using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Abstractions.Operations.Injectors
{
    /// <summary>
    /// Port for injecting Nintendo DS ROMs into Wii U Virtual Console packages.
    /// </summary>
    public interface INdsRomInjector
    {
        /// <summary>
        /// Injects a Nintendo DS ROM into the specified workspace.
        /// </summary>
        /// <remarks>
        /// The implementation must cast <paramref name="injection"/>.Options to <see cref="WiiUInjector.Domain.EmulatorOptions.DsOptions"/>
        /// to access console-specific settings.
        ///
        /// The ROM is read via <paramref name="injection"/>.Rom.Open() and the output is written inside
        /// <paramref name="workspace"/>.CodeDirectory or ContentDirectory as appropriate per workspace conventions.
        /// </remarks>
        /// <param name="injection">The complete injection aggregate containing console type, ROM source, and options.</param>
        /// <param name="workspace">The workspace providing paths for code, content, metadata, and temporary directories.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A Result indicating success or failure with application errors.</returns>
        Task<Result> InjectAsync(Injection injection, IInjectionWorkspace workspace, CancellationToken cancellationToken);
    }
}
