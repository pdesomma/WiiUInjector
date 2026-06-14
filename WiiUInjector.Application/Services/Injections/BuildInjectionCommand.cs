using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WiiUInjector.Domain;
using WiiUInjector.Domain.EmulatorOptions;

namespace WiiUInjector.Application.Services.Injections
{
    /// <summary>
    /// Sealed POCO carrying user inputs needed to build an Injection aggregate.
    /// </summary>
    public sealed class BuildInjectionCommand
    {
        /// <summary>
        /// The target console type for the injection.
        /// </summary>
        public ConsoleType Console { get; }

        /// <summary>
        /// The user's choice of base ROM (curated or custom).
        /// </summary>
        public BaseRomSelection BaseRomSelection { get; }

        /// <summary>
        /// Absolute or relative path to the ROM file; must not be null or whitespace.
        /// </summary>
        public string RomPath { get; }

        /// <summary>
        /// Paths to boot images, keyed by ImageSlot. Must contain all four slots (Icon, Drc, Tv, Logo).
        /// </summary>
        public IReadOnlyDictionary<ImageSlot, string> ImagePaths { get; }

        /// <summary>
        /// Boot screen metadata for the injection.
        /// </summary>
        public BootScreenMetadata BootScreen { get; }

        /// <summary>
        /// Console-specific emulator options; may be null (in which case BuildAsync uses EmulatorOptionsDefaults.For(Console)).
        /// </summary>
        public IEmulatorOptions Options { get; }

        /// <summary>
        /// Optional path to a boot sound file; may be null to omit boot sound.
        /// </summary>
        public string BootSoundPath { get; }

        /// <summary>
        /// Initializes a new BuildInjectionCommand with all required inputs.
        /// </summary>
        /// <param name="console">The target console type.</param>
        /// <param name="baseRomSelection">The base ROM selection (curated or custom).</param>
        /// <param name="romPath">Path to the ROM file; must not be null or whitespace.</param>
        /// <param name="imagePaths">Dictionary of image paths keyed by slot; must contain all four slots.</param>
        /// <param name="bootScreen">Boot screen metadata; must not be null.</param>
        /// <param name="options">Emulator options; may be null to use defaults.</param>
        /// <param name="bootSoundPath">Optional path to boot sound; may be null.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if baseRomSelection, romPath, imagePaths, or bootScreen is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if romPath is empty/whitespace or imagePaths does not contain all four slots or any slot path is empty/whitespace.
        /// </exception>
        public BuildInjectionCommand(
            ConsoleType console,
            BaseRomSelection baseRomSelection,
            string romPath,
            IReadOnlyDictionary<ImageSlot, string> imagePaths,
            BootScreenMetadata bootScreen,
            IEmulatorOptions options = null,
            string bootSoundPath = null)
        {
            if (baseRomSelection == null)
                throw new ArgumentNullException(nameof(baseRomSelection));

            if (romPath == null)
                throw new ArgumentNullException(nameof(romPath));

            if (string.IsNullOrWhiteSpace(romPath))
                throw new ArgumentException("RomPath cannot be empty.", nameof(romPath));

            if (imagePaths == null)
                throw new ArgumentNullException(nameof(imagePaths));

            if (bootScreen == null)
                throw new ArgumentNullException(nameof(bootScreen));

            // Check that all four slots are present and have non-empty paths.
            foreach (ImageSlot slot in new[] { ImageSlot.Icon, ImageSlot.Drc, ImageSlot.Tv, ImageSlot.Logo })
            {
                if (!imagePaths.ContainsKey(slot))
                    throw new ArgumentException($"ImagePaths must contain a path for slot {slot}.", nameof(imagePaths));

                string path = imagePaths[slot];
                if (path == null)
                    throw new ArgumentException($"ImagePaths[{slot}] cannot be null.", nameof(imagePaths));

                if (string.IsNullOrWhiteSpace(path))
                    throw new ArgumentException($"ImagePaths[{slot}] cannot be empty.", nameof(imagePaths));
            }

            Console = console;
            BaseRomSelection = baseRomSelection;
            RomPath = romPath;

            // Defensive copy the image paths dictionary.
            var imagePathsCopy = new Dictionary<ImageSlot, string>(imagePaths.Count);
            foreach (var kvp in imagePaths)
                imagePathsCopy[kvp.Key] = kvp.Value;

            ImagePaths = new ReadOnlyDictionary<ImageSlot, string>(imagePathsCopy);
            BootScreen = bootScreen;
            Options = options;
            BootSoundPath = bootSoundPath;
        }
    }
}
