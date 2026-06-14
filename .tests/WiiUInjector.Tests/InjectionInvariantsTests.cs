#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Domain;
using WiiUInjector.Domain.EmulatorOptions;

namespace WiiUInjector.Tests
{
    [TestClass]
    public class InjectionInvariantsTests
    {
        /// <summary>
        /// Ctor throws ArgumentNullException when baseRom is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullBaseRom_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Injection(
                ConsoleType.N64, null, TestData.AnyRom(), "rom.bin",
                TestData.AllImages(), TestData.AnyBootScreen(),
                new N64Options(false, false)));
        }

        /// <summary>
        /// Ctor throws ArgumentNullException when rom is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullRom_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Injection(
                ConsoleType.N64, TestData.AnyBase(), null, "rom.bin",
                TestData.AllImages(), TestData.AnyBootScreen(),
                new N64Options(false, false)));
        }

        /// <summary>
        /// Ctor throws ArgumentNullException when romFileName is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullRomFileName_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Injection(
                ConsoleType.N64, TestData.AnyBase(), TestData.AnyRom(), null,
                TestData.AllImages(), TestData.AnyBootScreen(),
                new N64Options(false, false)));
        }

        /// <summary>
        /// Ctor throws ArgumentException when romFileName is whitespace.
        /// </summary>
        [TestMethod]
        public void Ctor_WhitespaceRomFileName_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => new Injection(
                ConsoleType.N64, TestData.AnyBase(), TestData.AnyRom(), "   ",
                TestData.AllImages(), TestData.AnyBootScreen(),
                new N64Options(false, false)));
        }

        /// <summary>
        /// Ctor throws ArgumentNullException when images is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullImages_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Injection(
                ConsoleType.N64, TestData.AnyBase(), TestData.AnyRom(), "rom.bin",
                null, TestData.AnyBootScreen(),
                new N64Options(false, false)));
        }

        /// <summary>
        /// Ctor throws ArgumentNullException when bootScreen is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullBootScreen_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Injection(
                ConsoleType.N64, TestData.AnyBase(), TestData.AnyRom(), "rom.bin",
                TestData.AllImages(), null,
                new N64Options(false, false)));
        }

        /// <summary>
        /// Ctor throws ArgumentNullException when options is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullOptions_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new Injection(
                ConsoleType.N64, TestData.AnyBase(), TestData.AnyRom(), "rom.bin",
                TestData.AllImages(), TestData.AnyBootScreen(), null));
        }

        /// <summary>
        /// Ctor throws ArgumentException when options console does not match target console.
        /// </summary>
        [TestMethod]
        public void Ctor_OptionsConsoleMismatch_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => new Injection(
                ConsoleType.N64, TestData.AnyBase(ConsoleType.N64), TestData.AnyRom(), "rom.bin",
                TestData.AllImages(), TestData.AnyBootScreen(),
                new WiiOptions(false, false, false, false, false, false, false, false, false, ControllerLayout.GamePad, false)));
        }

        /// <summary>
        /// Ctor throws ArgumentException when images dict is missing a required slot.
        /// </summary>
        [TestMethod]
        public void Ctor_MissingImageSlot_Throws()
        {
            var images = new Dictionary<ImageSlot, GameImage>
            {
                { ImageSlot.Icon, TestData.IconImage() },
                { ImageSlot.Drc, TestData.DrcImage() },
                { ImageSlot.Tv, TestData.TvImage() }
            };
            Assert.ThrowsException<ArgumentException>(() => new Injection(
                ConsoleType.N64, TestData.AnyBase(), TestData.AnyRom(), "rom.bin",
                images, TestData.AnyBootScreen(),
                new N64Options(false, false)));
        }

        /// <summary>
        /// Ctor throws ArgumentException when GameImage.Slot does not match dict key.
        /// </summary>
        [TestMethod]
        public void Ctor_GameImageSlotMismatchesKey_Throws()
        {
            var images = new Dictionary<ImageSlot, GameImage>
            {
                { ImageSlot.Icon, TestData.IconImage() },
                { ImageSlot.Drc, TestData.IconImage() },
                { ImageSlot.Tv, TestData.TvImage() },
                { ImageSlot.Logo, TestData.LogoImage() }
            };
            Assert.ThrowsException<ArgumentException>(() => new Injection(
                ConsoleType.N64, TestData.AnyBase(), TestData.AnyRom(), "rom.bin",
                images, TestData.AnyBootScreen(),
                new N64Options(false, false)));
        }

        /// <summary>
        /// Ctor assigns all properties correctly with valid arguments.
        /// </summary>
        [TestMethod]
        public void Ctor_ValidArgs_AssignsAllProperties()
        {
            var console = ConsoleType.Gba;
            var baseRom = TestData.AnyBase(console);
            var rom = TestData.AnyRom(2048);
            var fileName = "game.gba";
            var images = TestData.AllImages();
            var bootScreen = TestData.AnyBootScreen();
            var options = new GbaOptions(true, false);

            var sut = new Injection(console, baseRom, rom, fileName, images, bootScreen, options);

            Assert.AreEqual(console, sut.Console);
            Assert.AreEqual(baseRom, sut.BaseRom);
            Assert.AreEqual(rom, sut.Rom);
            Assert.AreEqual(fileName, sut.RomFileName);
            Assert.AreEqual(4, sut.Images.Count);
            Assert.AreEqual(bootScreen, sut.BootScreen);
            Assert.AreEqual(options, sut.Options);
            Assert.IsNull(sut.BootSound);
        }

        /// <summary>
        /// Ctor allows null bootSound and assigns it correctly.
        /// </summary>
        [TestMethod]
        public void Ctor_OptionalBootSoundNull_AllowedAndAssignsNull()
        {
            var sut = BuildValid();
            Assert.IsNull(sut.BootSound);
        }

        /// <summary>
        /// WithOptions returns new instance with updated options, leaving original unchanged.
        /// </summary>
        [TestMethod]
        public void WithOptions_ValidNewOptions_ReturnsNewInstanceLeavingOriginalUnchanged()
        {
            var oldOptions = new WiiOptions(false, false, false, false, false, false, false, false, false, ControllerLayout.GamePad, false);
            var newOptions = new WiiOptions(true, true, false, false, false, false, false, false, false, ControllerLayout.GamePad, false);
            var sut = new Injection(ConsoleType.Wii, TestData.AnyBase(ConsoleType.Wii), TestData.AnyRom(), "rom.iso",
                TestData.AllImages(), TestData.AnyBootScreen(), oldOptions);

            var result = sut.WithOptions(newOptions);

            Assert.AreNotSame(sut, result);
            Assert.AreEqual(oldOptions, sut.Options);
            Assert.AreEqual(newOptions, result.Options);
        }

        /// <summary>
        /// WithOptions throws ArgumentException when new options console does not match.
        /// </summary>
        [TestMethod]
        public void WithOptions_ConsoleMismatch_Throws()
        {
            var sut = new Injection(ConsoleType.N64, TestData.AnyBase(ConsoleType.N64), TestData.AnyRom(), "rom.z64",
                TestData.AllImages(), TestData.AnyBootScreen(), new N64Options(false, false));

            Assert.ThrowsException<ArgumentException>(() => sut.WithOptions(
                new WiiOptions(false, false, false, false, false, false, false, false, false, ControllerLayout.GamePad, false)));
        }

        /// <summary>
        /// WithBootSound returns new instance with updated sound, leaving original unchanged.
        /// </summary>
        [TestMethod]
        public void WithBootSound_NewSound_ReturnsNewInstanceWithSound()
        {
            var sound1 = new BootSound(BootSoundFormat.Wav, new byte[] { 0x01 });
            var sound2 = new BootSound(BootSoundFormat.Wav, new byte[] { 0x02 });
            var sut = new Injection(ConsoleType.Wii, TestData.AnyBase(ConsoleType.Wii), TestData.AnyRom(), "rom.iso",
                TestData.AllImages(), TestData.AnyBootScreen(),
                new WiiOptions(false, false, false, false, false, false, false, false, false, ControllerLayout.GamePad, false),
                sound1);

            var result = sut.WithBootSound(sound2);

            Assert.AreNotSame(sut, result);
            Assert.AreEqual(sound1, sut.BootSound);
            Assert.AreEqual(sound2, result.BootSound);
        }

        /// <summary>
        /// WithBootSound returns new instance with null, leaving original unchanged.
        /// </summary>
        [TestMethod]
        public void WithBootSound_Null_ReturnsNewInstanceWithCleared()
        {
            var sound = new BootSound(BootSoundFormat.Wav, new byte[] { 0x01 });
            var sut = new Injection(ConsoleType.Wii, TestData.AnyBase(ConsoleType.Wii), TestData.AnyRom(), "rom.iso",
                TestData.AllImages(), TestData.AnyBootScreen(),
                new WiiOptions(false, false, false, false, false, false, false, false, false, ControllerLayout.GamePad, false),
                sound);

            var result = sut.WithBootSound(null);

            Assert.AreNotSame(sut, result);
            Assert.AreEqual(sound, sut.BootSound);
            Assert.IsNull(result.BootSound);
        }

        private static Injection BuildValid(ConsoleType console = ConsoleType.N64, IEmulatorOptions options = null)
            => new Injection(
                console,
                TestData.AnyBase(console),
                TestData.AnyRom(),
                "rom.bin",
                TestData.AllImages(),
                TestData.AnyBootScreen(),
                options ?? new N64Options(false, false));
    }

    /// <summary>
    /// Shared test data builders.
    /// </summary>
    internal static class TestData
    {
        /// <summary>
        /// Creates a valid Icon image (128x128x32).
        /// </summary>
        public static GameImage IconImage() => new GameImage(ImageSlot.Icon, 128, 128, 32, new byte[] { 0x01 });

        /// <summary>
        /// Creates a valid Drc image (854x480x24).
        /// </summary>
        public static GameImage DrcImage() => new GameImage(ImageSlot.Drc, 854, 480, 24, new byte[] { 0x01 });

        /// <summary>
        /// Creates a valid Tv image (1280x720x24).
        /// </summary>
        public static GameImage TvImage() => new GameImage(ImageSlot.Tv, 1280, 720, 24, new byte[] { 0x01 });

        /// <summary>
        /// Creates a valid Logo image (170x42x32).
        /// </summary>
        public static GameImage LogoImage() => new GameImage(ImageSlot.Logo, 170, 42, 32, new byte[] { 0x01 });

        /// <summary>
        /// Creates a complete image dict with all four slots.
        /// </summary>
        public static IReadOnlyDictionary<ImageSlot, GameImage> AllImages() => new Dictionary<ImageSlot, GameImage>
        {
            { ImageSlot.Icon, IconImage() },
            { ImageSlot.Drc, DrcImage() },
            { ImageSlot.Tv, TvImage() },
            { ImageSlot.Logo, LogoImage() }
        };

        /// <summary>
        /// Creates a BaseRom for the specified console.
        /// </summary>
        public static BaseRom AnyBase(ConsoleType console = ConsoleType.N64) =>
            new BaseRom("Test Base", "TEST0001", Region.Us, console, false);

        /// <summary>
        /// Creates a BootScreenMetadata.
        /// </summary>
        public static BootScreenMetadata AnyBootScreen() =>
            new BootScreenMetadata("Line1", "Line2", 2024, 1, "Long Name");

        /// <summary>
        /// Creates an IRomSource with the specified length.
        /// </summary>
        public static IRomSource AnyRom(long length = 1024) => new StubRomSource(length);

        /// <summary>
        /// Stub implementation of IRomSource for testing.
        /// </summary>
        public sealed class StubRomSource : IRomSource
        {
            public StubRomSource(long length) { Length = length; }
            public long Length { get; }
            public string ContentHash => "stub";
            public System.IO.Stream Open() => new System.IO.MemoryStream(new byte[1]);
        }
    }
}
