#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.IO;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;
using WiiUInjector.Domain.EmulatorOptions;

namespace WiiUInjector.Application.Tests
{
    /// <summary>
    /// Helper class providing canonical test data and factory methods for Application layer tests.
    /// </summary>
    internal static class TestData
    {
        /// <summary>
        /// Creates a valid Icon image (128x128x32).
        /// </summary>
        public static GameImage Icon128() => new GameImage(ImageSlot.Icon, 128, 128, 32, new byte[] { 0x01 });

        /// <summary>
        /// Creates a valid Drc image (854x480x24).
        /// </summary>
        public static GameImage Drc854() => new GameImage(ImageSlot.Drc, 854, 480, 24, new byte[] { 0x01 });

        /// <summary>
        /// Creates a valid Tv image (1280x720x24).
        /// </summary>
        public static GameImage Tv1280() => new GameImage(ImageSlot.Tv, 1280, 720, 24, new byte[] { 0x01 });

        /// <summary>
        /// Creates a valid Logo image (170x42x32).
        /// </summary>
        public static GameImage Logo170() => new GameImage(ImageSlot.Logo, 170, 42, 32, new byte[] { 0x01 });

        /// <summary>
        /// Creates a complete image dictionary with all four required slots.
        /// </summary>
        public static IReadOnlyDictionary<ImageSlot, GameImage> ImageDict(
            GameImage icon, GameImage drc, GameImage tv, GameImage logo)
        {
            return new ReadOnlyDictionary<ImageSlot, GameImage>(
                new Dictionary<ImageSlot, GameImage>
                {
                    { ImageSlot.Icon, icon },
                    { ImageSlot.Drc, drc },
                    { ImageSlot.Tv, tv },
                    { ImageSlot.Logo, logo }
                });
        }

        /// <summary>
        /// Creates a complete image dictionary with canonical dimensions.
        /// </summary>
        public static IReadOnlyDictionary<ImageSlot, GameImage> AllImages() =>
            ImageDict(Icon128(), Drc854(), Tv1280(), Logo170());

        /// <summary>
        /// Creates a BaseRom for the specified console.
        /// </summary>
        public static BaseRom AnyBaseRom(ConsoleType console = ConsoleType.N64) =>
            new BaseRom("Test Base", "TEST0001", Region.Us, console, false);

        /// <summary>
        /// Creates a BootScreenMetadata with sensible defaults.
        /// </summary>
        public static BootScreenMetadata BootScreen() =>
            new BootScreenMetadata("Line1", "Line2", 2024, 1, "Long Name");

        /// <summary>
        /// Creates an IRomSource with the specified length.
        /// </summary>
        public static IRomSource AnyRom(long length = 1024) => new StubRomSource(length);

        /// <summary>
        /// Creates a fully valid N64 injection using canonical test data.
        /// </summary>
        public static Injection AnyN64Injection() =>
            new Injection(
                ConsoleType.N64,
                AnyBaseRom(ConsoleType.N64),
                AnyRom(1024),
                "rom.z64",
                AllImages(),
                BootScreen(),
                new N64Options(false, false));

        /// <summary>
        /// Stub implementation of IRomSource for testing.
        /// </summary>
        public sealed class StubRomSource : IRomSource
        {
            private readonly long _length;

            /// <summary>
            /// Initializes a StubRomSource with the specified byte length.
            /// </summary>
            public StubRomSource(long length) => _length = length;

            /// <summary>
            /// Returns the configured length.
            /// </summary>
            public long Length => _length;

            /// <summary>
            /// Returns a fixed content hash.
            /// </summary>
            public string ContentHash => "stub_content_hash";

            /// <summary>
            /// Returns a MemoryStream over a single-byte buffer.
            /// </summary>
            public Stream Open() => new MemoryStream(new byte[1]);
        }
    }
}
