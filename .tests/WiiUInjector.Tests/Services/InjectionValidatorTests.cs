#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Domain;
using WiiUInjector.Domain.EmulatorOptions;
using WiiUInjector.Domain.Services;
using static WiiUInjector.Tests.TestData;

namespace WiiUInjector.Tests.Services
{
    [TestClass]
    public class InjectionValidatorTests
    {
        private InjectionValidator sut;

        [TestInitialize]
        public void Setup() => sut = new InjectionValidator();

        /// <summary>
        /// Validate throws ArgumentNullException when injection is null.
        /// </summary>
        [TestMethod]
        public void Validate_NullInjection_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => sut.Validate(null));
        }

        /// <summary>
        /// Validate returns success when all rules pass.
        /// </summary>
        [TestMethod]
        public void Validate_HappyPath_AllRulesPass_ReturnsIsValid()
        {
            var injection = new Injection(
                ConsoleType.N64,
                AnyBase(ConsoleType.N64),
                AnyRom(1024),
                "rom.z64",
                AllImages(),
                AnyBootScreen(),
                new N64Options(false, false));

            var result = sut.Validate(injection);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        /// <summary>
        /// Validate adds error when Icon dimensions are wrong.
        /// </summary>
        [TestMethod]
        public void Validate_WrongIconDimensions_AddsIconError()
        {
            var images = new Dictionary<ImageSlot, GameImage>
            {
                { ImageSlot.Icon, new GameImage(ImageSlot.Icon, 64, 64, 32, new byte[] { 0x01 }) },
                { ImageSlot.Drc, DrcImage() },
                { ImageSlot.Tv, TvImage() },
                { ImageSlot.Logo, LogoImage() }
            };
            var injection = new Injection(ConsoleType.N64, AnyBase(ConsoleType.N64), AnyRom(), "rom.z64",
                images, AnyBootScreen(), new N64Options(false, false));

            var result = sut.Validate(injection);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count >= 1);
            Assert.IsTrue(HasError(result, "ICON_DIMENSIONS"));
        }

        /// <summary>
        /// Validate adds error when Drc dimensions are wrong.
        /// </summary>
        [TestMethod]
        public void Validate_WrongDrcDimensions_AddsDrcError()
        {
            var images = new Dictionary<ImageSlot, GameImage>
            {
                { ImageSlot.Icon, IconImage() },
                { ImageSlot.Drc, new GameImage(ImageSlot.Drc, 640, 480, 24, new byte[] { 0x01 }) },
                { ImageSlot.Tv, TvImage() },
                { ImageSlot.Logo, LogoImage() }
            };
            var injection = new Injection(ConsoleType.N64, AnyBase(ConsoleType.N64), AnyRom(), "rom.z64",
                images, AnyBootScreen(), new N64Options(false, false));

            var result = sut.Validate(injection);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(HasError(result, "DRC_DIMENSIONS"));
        }

        /// <summary>
        /// Validate adds error when Tv dimensions are wrong.
        /// </summary>
        [TestMethod]
        public void Validate_WrongTvDimensions_AddsTvError()
        {
            var images = new Dictionary<ImageSlot, GameImage>
            {
                { ImageSlot.Icon, IconImage() },
                { ImageSlot.Drc, DrcImage() },
                { ImageSlot.Tv, new GameImage(ImageSlot.Tv, 1024, 720, 24, new byte[] { 0x01 }) },
                { ImageSlot.Logo, LogoImage() }
            };
            var injection = new Injection(ConsoleType.N64, AnyBase(ConsoleType.N64), AnyRom(), "rom.z64",
                images, AnyBootScreen(), new N64Options(false, false));

            var result = sut.Validate(injection);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(HasError(result, "TV_DIMENSIONS"));
        }

        /// <summary>
        /// Validate adds error when Logo dimensions are wrong.
        /// </summary>
        [TestMethod]
        public void Validate_WrongLogoDimensions_AddsLogoError()
        {
            var images = new Dictionary<ImageSlot, GameImage>
            {
                { ImageSlot.Icon, IconImage() },
                { ImageSlot.Drc, DrcImage() },
                { ImageSlot.Tv, TvImage() },
                { ImageSlot.Logo, new GameImage(ImageSlot.Logo, 100, 42, 32, new byte[] { 0x01 }) }
            };
            var injection = new Injection(ConsoleType.N64, AnyBase(ConsoleType.N64), AnyRom(), "rom.z64",
                images, AnyBootScreen(), new N64Options(false, false));

            var result = sut.Validate(injection);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(HasError(result, "LOGO_DIMENSIONS"));
        }

        /// <summary>
        /// Validate adds error when Icon bit depth is wrong.
        /// </summary>
        [TestMethod]
        public void Validate_WrongIconBitDepth_AddsIconError()
        {
            var images = new Dictionary<ImageSlot, GameImage>
            {
                { ImageSlot.Icon, new GameImage(ImageSlot.Icon, 128, 128, 24, new byte[] { 0x01 }) },
                { ImageSlot.Drc, DrcImage() },
                { ImageSlot.Tv, TvImage() },
                { ImageSlot.Logo, LogoImage() }
            };
            var injection = new Injection(ConsoleType.N64, AnyBase(ConsoleType.N64), AnyRom(), "rom.z64",
                images, AnyBootScreen(), new N64Options(false, false));

            var result = sut.Validate(injection);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(HasError(result, "ICON_DIMENSIONS"));
        }

        /// <summary>
        /// Validate adds error when ROM exceeds size cap for console.
        /// </summary>
        [TestMethod]
        public void Validate_RomTooLarge_AddsRomSizeError()
        {
            var largeSizeBytes = 100L * 1024 * 1024;
            var injection = new Injection(
                ConsoleType.N64,
                AnyBase(ConsoleType.N64),
                AnyRom(largeSizeBytes),
                "rom.z64",
                AllImages(),
                AnyBootScreen(),
                new N64Options(false, false));

            var result = sut.Validate(injection);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(HasError(result, "ROM_SIZE"));
        }

        /// <summary>
        /// Validate passes when ROM is within size cap.
        /// </summary>
        [TestMethod]
        public void Validate_RomWithinCap_NoRomSizeError()
        {
            var sizeBytes = 63L * 1024 * 1024;
            var injection = new Injection(
                ConsoleType.N64,
                AnyBase(ConsoleType.N64),
                AnyRom(sizeBytes),
                "rom.z64",
                AllImages(),
                AnyBootScreen(),
                new N64Options(false, false));

            var result = sut.Validate(injection);

            Assert.IsTrue(result.IsValid);
            Assert.IsFalse(HasError(result, "ROM_SIZE"));
        }

        /// <summary>
        /// Validate adds error when Wii ToPal is set without VideoModePatch.
        /// </summary>
        [TestMethod]
        public void Validate_WiiToPalWithoutVideoPatch_AddsError()
        {
            var options = WiiOpts(toPal: true, videoPatch: false);
            var injection = new Injection(
                ConsoleType.Wii,
                AnyBase(ConsoleType.Wii),
                AnyRom(),
                "rom.iso",
                AllImages(),
                AnyBootScreen(),
                options);

            var result = sut.Validate(injection);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(HasError(result, "WII_TO_PAL_REQUIRES_VIDEO_PATCH"));
        }

        /// <summary>
        /// Validate passes when Wii ToPal is set with VideoModePatch.
        /// </summary>
        [TestMethod]
        public void Validate_WiiToPalWithVideoPatch_NoError()
        {
            var options = WiiOpts(toPal: true, videoPatch: true);
            var injection = new Injection(
                ConsoleType.Wii,
                AnyBase(ConsoleType.Wii),
                AnyRom(),
                "rom.iso",
                AllImages(),
                AnyBootScreen(),
                options);

            var result = sut.Validate(injection);

            Assert.IsTrue(result.IsValid);
            Assert.IsFalse(HasError(result, "WII_TO_PAL_REQUIRES_VIDEO_PATCH"));
        }

        /// <summary>
        /// Validate adds error when Wii RegionFreeAll conflicts with RegionFreeUs.
        /// </summary>
        [TestMethod]
        public void Validate_WiiRegionFreeAllWithUs_AddsConflictError()
        {
            var options = WiiOpts(regionFreeAll: true, regionFreeUs: true);
            var injection = new Injection(
                ConsoleType.Wii,
                AnyBase(ConsoleType.Wii),
                AnyRom(),
                "rom.iso",
                AllImages(),
                AnyBootScreen(),
                options);

            var result = sut.Validate(injection);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(HasError(result, "WII_REGIONFREE_CONFLICT"));
        }

        /// <summary>
        /// Validate adds error when Wii RegionFreeAll conflicts with RegionFreeJp.
        /// </summary>
        [TestMethod]
        public void Validate_WiiRegionFreeAllWithJp_AddsConflictError()
        {
            var options = WiiOpts(regionFreeAll: true, regionFreeJp: true);
            var injection = new Injection(
                ConsoleType.Wii,
                AnyBase(ConsoleType.Wii),
                AnyRom(),
                "rom.iso",
                AllImages(),
                AnyBootScreen(),
                options);

            var result = sut.Validate(injection);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(HasError(result, "WII_REGIONFREE_CONFLICT"));
        }

        /// <summary>
        /// Validate reports multiple errors when multiple rules are violated.
        /// </summary>
        [TestMethod]
        public void Validate_MultipleErrors_AllReported()
        {
            var images = new Dictionary<ImageSlot, GameImage>
            {
                { ImageSlot.Icon, new GameImage(ImageSlot.Icon, 64, 64, 32, new byte[] { 0x01 }) },
                { ImageSlot.Drc, DrcImage() },
                { ImageSlot.Tv, TvImage() },
                { ImageSlot.Logo, LogoImage() }
            };
            var largeSizeBytes = 100L * 1024 * 1024;
            var injection = new Injection(
                ConsoleType.N64,
                AnyBase(ConsoleType.N64),
                AnyRom(largeSizeBytes),
                "rom.z64",
                images,
                AnyBootScreen(),
                new N64Options(false, false));

            var result = sut.Validate(injection);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Count >= 2);
            Assert.IsTrue(HasError(result, "ICON_DIMENSIONS"));
            Assert.IsTrue(HasError(result, "ROM_SIZE"));
        }

        private static WiiOptions WiiOpts(
            bool removeDeflicker = false, bool removeDithering = false, bool halfV = false,
            bool videoPatch = false, bool toPal = false,
            bool regionFreeUs = false, bool regionFreeJp = false, bool regionFreeAll = false,
            bool motionPassthrough = false, ControllerLayout layout = ControllerLayout.GamePad,
            bool lrPatch = false)
            => new WiiOptions(removeDeflicker, removeDithering, halfV, videoPatch, toPal,
                              regionFreeUs, regionFreeJp, regionFreeAll, motionPassthrough, layout, lrPatch);

        private static bool HasError(ValidationResult result, string code)
        {
            foreach (var err in result.Errors)
                if (err.Code == code)
                    return true;
            return false;
        }
    }
}
