using System;

namespace WiiUInjector.Application.Abstractions.Operations
{
    /// <summary>
    /// Represents a temporary workspace for injection operations.
    /// Provides typed paths for organizing base ROM, content, metadata, and temporary files
    /// following the directory layout convention: root → {code, content, meta, temp} subdirectories.
    /// Disposing the workspace deletes the root directory and all contents.
    /// </summary>
    public interface IInjectionWorkspace : IDisposable
    {
        /// <summary>
        /// Gets the root directory path of this workspace.
        /// </summary>
        string RootDirectory { get; }

        /// <summary>
        /// Gets the code subdirectory path (for executable binaries and scripts).
        /// Equivalent to RootDirectory + "\code".
        /// </summary>
        string CodeDirectory { get; }

        /// <summary>
        /// Gets the content subdirectory path (for ROM data, game ISO, NFS files).
        /// Equivalent to RootDirectory + "\content".
        /// </summary>
        string ContentDirectory { get; }

        /// <summary>
        /// Gets the metadata subdirectory path (for meta.xml, app.xml, bootSound.btsnd).
        /// Equivalent to RootDirectory + "\meta".
        /// </summary>
        string MetaDirectory { get; }

        /// <summary>
        /// Gets the temporary subdirectory path (for intermediate conversion outputs).
        /// Equivalent to RootDirectory + "\temp".
        /// </summary>
        string TempDirectory { get; }
    }
}
