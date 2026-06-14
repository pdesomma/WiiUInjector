using System;

namespace WiiUInjector.Application.Services.Injections
{
    /// <summary>
    /// Enumeration of supported output formats for ROM injection operations.
    /// </summary>
    public enum InjectionOutputFormat
    {
        /// <summary>
        /// Loadiine format output for use with Loadiine-compatible loaders.
        /// </summary>
        Loadiine,

        /// <summary>
        /// Packed format output (typically for disc-based distribution or installation).
        /// </summary>
        Packed
    }
}
