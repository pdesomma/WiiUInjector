#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Application.Abstractions.IO;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.Injections;
using WiiUInjector.Application.Tests.Fakes;
using WiiUInjector.Application.Tests.FakesB;
using WiiUInjector.Domain;
using WiiUInjector.Domain.EmulatorOptions;
using WiiUInjector.Domain.Services;

namespace WiiUInjector.Application.Tests.Services.Injections
{
    /// <summary>
    /// Tests for InjectionService.BuildAsync.
    /// </summary>
    [TestClass]
    public class InjectionService_BuildAsyncTests
    {
        private InjectionService sut;
        private FakeRomFileLoader fakeRomLoader;
        private FakeGameImageLoader fakeImageLoader;
        private FakeBootSoundLoader fakeSoundLoader;
        private FakesB.FakeBaseRomCatalog fakeBaseRomCatalog;
        private FakeInjectionValidator fakeValidator;
        private FakeInjectionPipeline fakePipeline;
        private FakeApplicationLogger fakeLogger;

        [TestInitialize]
        public void Setup()
        {
            fakeRomLoader = new FakeRomFileLoader();
            fakeImageLoader = new FakeGameImageLoader();
            fakeSoundLoader = new FakeBootSoundLoader();
            fakeBaseRomCatalog = new FakesB.FakeBaseRomCatalog();
            fakeBaseRomCatalog.NextFindResult = Result<BaseRom>.Success(TestData.AnyBaseRom());
            fakeBaseRomCatalog.NextLoadCustomResult = Result<BaseRom>.Success(TestData.AnyBaseRom());
            fakeValidator = new FakeInjectionValidator();
            fakePipeline = new FakeInjectionPipeline();
            fakeLogger = new FakeApplicationLogger();

            sut = new InjectionService(
                fakeRomLoader,
                fakeImageLoader,
                fakeSoundLoader,
                fakeBaseRomCatalog,
                fakeValidator,
                fakePipeline,
                fakeLogger);
        }

        /// <summary>
        /// BuildAsync returns failure when command is null.
        /// </summary>
        [TestMethod]
        public async Task BuildAsync_NullCommand_ReturnsFailure()
        {
            var result = await sut.BuildAsync(null, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(ApplicationErrors.DomainInvariantViolation, result.Errors[0].Code);
        }

        /// <summary>
        /// BuildAsync with curated base returns success and calls FindCuratedAsync exactly once.
        /// </summary>
        [TestMethod]
        public async Task BuildAsync_CuratedBase_CallsFindCuratedAsync()
        {
            var baseRomSelection = BaseRomSelection.Curated("TITLE0001", Region.Us);
            var cmd = new BuildInjectionCommand(
                ConsoleType.N64,
                baseRomSelection,
                "test.z64",
                AllImagePaths(),
                TestData.BootScreen());

            var result = await sut.BuildAsync(cmd, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(1, fakeBaseRomCatalog.FindCallCount);
            Assert.AreEqual(0, fakeBaseRomCatalog.LoadCustomCallCount);
        }

        /// <summary>
        /// BuildAsync with custom base returns success and calls LoadCustomAsync exactly once.
        /// </summary>
        [TestMethod]
        public async Task BuildAsync_CustomBase_CallsLoadCustomAsync()
        {
            var baseRomSelection = BaseRomSelection.Custom("C:\\custom_base.wua");
            var cmd = new BuildInjectionCommand(
                ConsoleType.N64,
                baseRomSelection,
                "test.z64",
                AllImagePaths(),
                TestData.BootScreen());

            var result = await sut.BuildAsync(cmd, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, fakeBaseRomCatalog.FindCallCount);
            Assert.AreEqual(1, fakeBaseRomCatalog.LoadCustomCallCount);
        }

        /// <summary>
        /// BuildAsync returns failure when ROM loader fails.
        /// </summary>
        [TestMethod]
        public async Task BuildAsync_RomLoadFails_ReturnsFailure()
        {
            fakeRomLoader.NextResult = Result<RomLoadResult>.Failure(
                new ApplicationError(ApplicationErrors.RomNotFound, "ROM not found."));

            var cmd = BuildCommand();
            var result = await sut.BuildAsync(cmd, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(ApplicationErrors.RomNotFound, result.Errors[0].Code);
        }

        /// <summary>
        /// BuildAsync returns failure when any image loader fails.
        /// </summary>
        [TestMethod]
        public async Task BuildAsync_ImageLoadFails_ReturnsFailure()
        {
            fakeImageLoader.Results[ImageSlot.Icon] = Result<GameImage>.Failure(
                new ApplicationError(ApplicationErrors.ImageNotFound, "Icon not found."));

            var cmd = BuildCommand();
            var result = await sut.BuildAsync(cmd, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.IsTrue(result.Errors.Count >= 1);
            Assert.AreEqual(ApplicationErrors.ImageNotFound, result.Errors[0].Code);
        }

        /// <summary>
        /// BuildAsync aggregates multiple image loader failures.
        /// </summary>
        [TestMethod]
        public async Task BuildAsync_MultipleImageLoadFails_AggregatesErrors()
        {
            fakeImageLoader.Results[ImageSlot.Icon] = Result<GameImage>.Failure(
                new ApplicationError(ApplicationErrors.ImageNotFound, "Icon not found."));
            fakeImageLoader.Results[ImageSlot.Drc] = Result<GameImage>.Failure(
                new ApplicationError(ApplicationErrors.ImageNotFound, "Drc not found."));

            var cmd = BuildCommand();
            var result = await sut.BuildAsync(cmd, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(2, result.Errors.Count);
        }

        /// <summary>
        /// BuildAsync skips boot sound loader when BootSoundPath is null.
        /// </summary>
        [TestMethod]
        public async Task BuildAsync_NullBootSoundPath_SkipsLoader()
        {
            var cmd = new BuildInjectionCommand(
                ConsoleType.N64,
                BaseRomSelection.Curated("TITLE0001", Region.Us),
                "test.z64",
                AllImagePaths(),
                TestData.BootScreen(),
                null,
                null);

            var result = await sut.BuildAsync(cmd, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNull(result.Value.BootSound);
            Assert.AreEqual(0, fakeSoundLoader.CallCount);
        }

        /// <summary>
        /// BuildAsync returns failure when boot sound loader fails.
        /// </summary>
        [TestMethod]
        public async Task BuildAsync_BootSoundLoadFails_ReturnsFailure()
        {
            fakeSoundLoader.NextResult = Result<BootSound>.Failure(
                new ApplicationError(ApplicationErrors.BootSoundNotFound, "Sound not found."));

            var cmd = new BuildInjectionCommand(
                ConsoleType.N64,
                BaseRomSelection.Curated("TITLE0001", Region.Us),
                "test.z64",
                AllImagePaths(),
                TestData.BootScreen(),
                null,
                "sound.mp3");

            var result = await sut.BuildAsync(cmd, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.BootSoundNotFound, result.Errors[0].Code);
        }

        /// <summary>
        /// BuildAsync uses default emulator options when options parameter is null.
        /// </summary>
        [TestMethod]
        public async Task BuildAsync_NullOptions_UsesDefaults()
        {
            var cmd = new BuildInjectionCommand(
                ConsoleType.N64,
                BaseRomSelection.Curated("TITLE0001", Region.Us),
                "test.z64",
                AllImagePaths(),
                TestData.BootScreen(),
                null,
                null);

            var result = await sut.BuildAsync(cmd, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value.Options);
            Assert.AreEqual(ConsoleType.N64, result.Value.Options.Console);
        }

        /// <summary>
        /// BuildAsync returns failure when options console does not match command console.
        /// </summary>
        [TestMethod]
        public async Task BuildAsync_OptionConsoleMismatch_ReturnsFailure()
        {
            var wiiOptions = new WiiOptions(false, false, false, false, false, false, false, false, false, ControllerLayout.GamePad, false);
            var cmd = new BuildInjectionCommand(
                ConsoleType.N64,
                BaseRomSelection.Curated("TITLE0001", Region.Us),
                "test.z64",
                AllImagePaths(),
                TestData.BootScreen(),
                wiiOptions,
                null);

            var result = await sut.BuildAsync(cmd, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.EmulatorOptionsConsoleMismatch, result.Errors[0].Code);
        }

        /// <summary>
        /// BuildAsync calls validator exactly once with constructed injection.
        /// </summary>
        [TestMethod]
        public async Task BuildAsync_Success_CallsValidatorOnce()
        {
            var cmd = BuildCommand();
            var result = await sut.BuildAsync(cmd, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, fakeValidator.CallCount);
            Assert.IsNotNull(fakeValidator.LastInjection);
        }

        /// <summary>
        /// BuildAsync returns mapped domain validation errors on failure.
        /// </summary>
        [TestMethod]
        public async Task BuildAsync_ValidatorFails_MapsErrors()
        {
            var validationErrors = new List<ValidationError>
            {
                new ValidationError("ICON_DIMENSIONS", "Icon wrong size", "Images[Icon]")
            };
            fakeValidator.NextResult = new ValidationResult(validationErrors);

            var cmd = BuildCommand();
            var result = await sut.BuildAsync(cmd, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("ICON_DIMENSIONS", result.Errors[0].Code);
            Assert.AreEqual("Icon wrong size", result.Errors[0].Message);
        }

        /// <summary>
        /// BuildAsync returns success with valid injection when all preconditions pass.
        /// </summary>
        [TestMethod]
        public async Task BuildAsync_HappyPath_ReturnsValidInjection()
        {
            var cmd = BuildCommand();
            var result = await sut.BuildAsync(cmd, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(ConsoleType.N64, result.Value.Console);
            Assert.IsNotNull(result.Value.BaseRom);
            Assert.IsNotNull(result.Value.Rom);
            Assert.AreEqual(4, result.Value.Images.Count);
            Assert.IsNotNull(result.Value.Options);
        }

        /// <summary>
        /// BuildAsync returns failure when base ROM catalog fails.
        /// </summary>
        [TestMethod]
        public async Task BuildAsync_BaseRomCatalogFails_ReturnsFailure()
        {
            fakeBaseRomCatalog.NextFindResult = Result<BaseRom>.Failure(
                new ApplicationError(ApplicationErrors.BaseRomNotInCatalog, "Base ROM not found."));

            var cmd = BuildCommand();
            var result = await sut.BuildAsync(cmd, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.BaseRomNotInCatalog, result.Errors[0].Code);
        }

        // Helper methods
        private BuildInjectionCommand BuildCommand()
        {
            return new BuildInjectionCommand(
                ConsoleType.N64,
                BaseRomSelection.Curated("TITLE0001", Region.Us),
                "test.z64",
                AllImagePaths(),
                TestData.BootScreen());
        }

        private IReadOnlyDictionary<ImageSlot, string> AllImagePaths()
        {
            return new ReadOnlyDictionary<ImageSlot, string>(
                new Dictionary<ImageSlot, string>
                {
                    { ImageSlot.Icon, "icon.tga" },
                    { ImageSlot.Drc, "drc.tga" },
                    { ImageSlot.Tv, "tv.tga" },
                    { ImageSlot.Logo, "logo.tga" }
                });
        }
    }
}
