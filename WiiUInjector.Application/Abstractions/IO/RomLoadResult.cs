using System;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Abstractions.IO
{
    /// <summary>
    /// Immutable value object containing the result of loading a ROM file.
    /// </summary>
    public sealed class RomLoadResult
    {
        /// <summary>
        /// The loaded ROM source providing lazy access to ROM bytes.
        /// </summary>
        public IRomSource Rom { get; }

        /// <summary>
        /// The file name (not full path) of the loaded ROM.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Constructs a RomLoadResult with a ROM source and file name.
        /// </summary>
        /// <param name="rom">The loaded ROM source; cannot be null.</param>
        /// <param name="fileName">The file name; cannot be null or empty.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rom"/> or <paramref name="fileName"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="fileName"/> is empty or whitespace.</exception>
        public RomLoadResult(IRomSource rom, string fileName)
        {
            if (rom == null)
                throw new ArgumentNullException(nameof(rom));

            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("FileName cannot be empty.", nameof(fileName));

            Rom = rom;
            FileName = fileName;
        }
    }
}
