using System.IO;

namespace WiiUInjector.Domain
{
    /// <summary>
    /// Lazy accessor for ROM bytes. Implementations may stream from disk, memory, or a network source.
    /// </summary>
    public interface IRomSource
    {
        /// <summary>
        /// Opens a fresh readable stream positioned at byte 0. The caller is responsible for disposing it.
        /// </summary>
        /// <returns>A new readable stream over the ROM payload.</returns>
        Stream Open();

        /// <summary>
        /// Total byte length of the ROM payload.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Stable content hash (algorithm chosen by the implementation, e.g. SHA-256 hex string).
        /// Used for integrity validation and deduplication.
        /// </summary>
        string ContentHash { get; }
    }
}
