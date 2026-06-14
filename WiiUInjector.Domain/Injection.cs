using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WiiUInjector.Domain.EmulatorOptions;

namespace WiiUInjector.Domain
{
    /// <summary>
    /// Aggregate root representing a complete ROM injection specification for a Wii U Virtual Console base package.
    /// Encapsulates console type, base ROM, ROM source, images, boot metadata, and emulator options.
    /// </summary>
    public sealed class Injection
    {
        /// <summary>
        /// The target console type for this injection.
        /// </summary>
        public ConsoleType Console { get; }

        /// <summary>
        /// The base ROM package metadata.
        /// </summary>
        public BaseRom BaseRom { get; }

        /// <summary>
        /// The ROM source (provides access to ROM data).
        /// </summary>
        public IRomSource Rom { get; }

        /// <summary>
        /// The name of the ROM file (e.g., "game.gba").
        /// </summary>
        public string RomFileName { get; }

        /// <summary>
        /// All required boot images, keyed by their slot (Icon, Drc, Tv, Logo).
        /// </summary>
        public IReadOnlyDictionary<ImageSlot, GameImage> Images { get; }

        /// <summary>
        /// Boot screen metadata.
        /// </summary>
        public BootScreenMetadata BootScreen { get; }

        /// <summary>
        /// Console-specific emulator options.
        /// </summary>
        public IEmulatorOptions Options { get; }

        /// <summary>
        /// Optional boot sound (may be null).
        /// </summary>
        public BootSound BootSound { get; }

        /// <summary>
        /// Initializes a new Injection with the specified console configuration, ROM, images, and emulator options.
        /// </summary>
        /// <param name="console">The target console type.</param>
        /// <param name="baseRom">The base ROM package.</param>
        /// <param name="rom">The ROM source.</param>
        /// <param name="romFileName">The ROM file name (must not be null or whitespace).</param>
        /// <param name="images">Dictionary of required images keyed by slot; must include all four slots.</param>
        /// <param name="bootScreen">Boot screen metadata.</param>
        /// <param name="options">Emulator options for the target console.</param>
        /// <param name="bootSound">Optional boot sound (may be null).</param>
        public Injection(
            ConsoleType console,
            BaseRom baseRom,
            IRomSource rom,
            string romFileName,
            IReadOnlyDictionary<ImageSlot, GameImage> images,
            BootScreenMetadata bootScreen,
            IEmulatorOptions options,
            BootSound bootSound = null)
        {
            if (baseRom == null) throw new ArgumentNullException(nameof(baseRom));
            if (rom == null) throw new ArgumentNullException(nameof(rom));
            if (romFileName == null) throw new ArgumentNullException(nameof(romFileName));
            if (string.IsNullOrWhiteSpace(romFileName)) throw new ArgumentException("RomFileName cannot be empty.", nameof(romFileName));
            if (images == null) throw new ArgumentNullException(nameof(images));
            if (bootScreen == null) throw new ArgumentNullException(nameof(bootScreen));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (options.Console != console) throw new ArgumentException($"Options are for {options.Console} but injection targets {console}.", nameof(options));

            foreach (ImageSlot slot in new[] { ImageSlot.Icon, ImageSlot.Drc, ImageSlot.Tv, ImageSlot.Logo })
                if (!images.ContainsKey(slot))
                    throw new ArgumentException($"Missing required image slot: {slot}.", nameof(images));

            foreach (var kvp in images)
                if (kvp.Value.Slot != kvp.Key)
                    throw new ArgumentException($"Image at slot key {kvp.Key} has mismatched Slot {kvp.Value.Slot}.", nameof(images));

            Console = console;
            BaseRom = baseRom;
            Rom = rom;
            RomFileName = romFileName;

            var copy = new Dictionary<ImageSlot, GameImage>(images.Count);
            foreach (var kvp in images) copy[kvp.Key] = kvp.Value;
            Images = new ReadOnlyDictionary<ImageSlot, GameImage>(copy);

            BootScreen = bootScreen;
            Options = options;
            BootSound = bootSound;
        }

        /// <summary>
        /// Returns a new Injection with the specified emulator options, running all constructor invariants.
        /// </summary>
        /// <param name="newOptions">The new emulator options.</param>
        /// <returns>A new Injection instance.</returns>
        public Injection WithOptions(IEmulatorOptions newOptions)
        {
            return new Injection(Console, BaseRom, Rom, RomFileName, Images, BootScreen, newOptions, BootSound);
        }

        /// <summary>
        /// Returns a new Injection with the specified boot sound.
        /// </summary>
        /// <param name="sound">The new boot sound (may be null to clear).</param>
        /// <returns>A new Injection instance.</returns>
        public Injection WithBootSound(BootSound sound)
        {
            return new Injection(Console, BaseRom, Rom, RomFileName, Images, BootScreen, Options, sound);
        }
    }
}
