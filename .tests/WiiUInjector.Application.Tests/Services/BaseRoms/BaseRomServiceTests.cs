#nullable disable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.BaseRoms;
using WiiUInjector.Application.Tests.Fakes;
using WiiUInjector.Application.Tests.FakesB;
using FakeApplicationLogger = WiiUInjector.Application.Tests.Fakes.FakeApplicationLogger;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Tests.Services.BaseRoms
{
    [TestClass]
    public class BaseRomServiceTests
    {
        private BaseRomService sut;
        private FakeBaseRomCatalog catalog;
        private FakeApplicationLogger logger;

        [TestInitialize]
        public void Setup()
        {
            catalog = new FakeBaseRomCatalog();
            logger = new FakeApplicationLogger();
            sut = new BaseRomService(catalog, logger);
        }

        /// <summary>
        /// Ctor throws when catalog is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullCatalog_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BaseRomService(null, logger));
        }

        /// <summary>
        /// Ctor throws when logger is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullLogger_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BaseRomService(catalog, null));
        }

        /// <summary>
        /// ListAsync delegates to catalog and logs success.
        /// </summary>
        [TestMethod]
        public async Task ListAsync_HappyPath_ReturnsListAndLogsInfo()
        {
            var base1 = new BaseRom("Base 1", "0000000000000001", Region.Us, ConsoleType.Wii, false);
            var base2 = new BaseRom("Base 2", "0000000000000002", Region.Eu, ConsoleType.Wii, false);
            var base3 = new BaseRom("Base 3", "0000000000000003", Region.Jp, ConsoleType.Wii, false);
            var expectedBases = new List<BaseRom> { base1, base2, base3 };

            catalog.NextListResult = Result<IReadOnlyList<BaseRom>>.Success(expectedBases);

            var result = await sut.ListAsync(ConsoleType.Wii, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(3, result.Value.Count);
            Assert.AreEqual(1, catalog.ListCallCount);
            Assert.AreEqual(ConsoleType.Wii, catalog.LastListConsole);
            Assert.AreEqual(1, logger.InfoMessages.Count);
            Assert.IsTrue(logger.InfoMessages[0].Contains("3"));
        }

        /// <summary>
        /// ListAsync logs warning on failure.
        /// </summary>
        [TestMethod]
        public async Task ListAsync_CatalogFailure_PropagatesToCallerAndLogsWarning()
        {
            var error = new ApplicationError(ApplicationErrors.BaseRomNotInCatalog, "Base not found.", null);
            catalog.NextListResult = Result<IReadOnlyList<BaseRom>>.Failure(error);

            var result = await sut.ListAsync(ConsoleType.Wii, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual(ApplicationErrors.BaseRomNotInCatalog, result.Errors[0].Code);
            Assert.AreEqual(1, logger.WarnMessages.Count);
        }

        /// <summary>
        /// FindCuratedAsync with null titleId returns boundary failure.
        /// </summary>
        [TestMethod]
        public async Task FindCuratedAsync_NullTitleId_ReturnsBoundaryFailure()
        {
            var result = await sut.FindCuratedAsync(ConsoleType.Wii, null, Region.Us, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.BaseRomNotInCatalog, result.Errors[0].Code);
            Assert.AreEqual(0, catalog.FindCallCount);
        }

        /// <summary>
        /// FindCuratedAsync delegates to catalog and logs success.
        /// </summary>
        [TestMethod]
        public async Task FindCuratedAsync_HappyPath_DelegatesAndLogsInfo()
        {
            var baseRom = new BaseRom("Test Base", "0000000000000001", Region.Us, ConsoleType.Wii, false);
            catalog.NextFindResult = Result<BaseRom>.Success(baseRom);

            var result = await sut.FindCuratedAsync(ConsoleType.Wii, "0000000000000001", Region.Us, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(baseRom, result.Value);
            Assert.AreEqual(1, catalog.FindCallCount);
            Assert.AreEqual(1, logger.InfoMessages.Count);
        }

        /// <summary>
        /// FindCuratedAsync propagates catalog failure.
        /// </summary>
        [TestMethod]
        public async Task FindCuratedAsync_CatalogFailure_PropagatesToCallerAndLogsWarning()
        {
            var error = new ApplicationError(ApplicationErrors.BaseRomNotInCatalog, "Not found.", null);
            catalog.NextFindResult = Result<BaseRom>.Failure(error);

            var result = await sut.FindCuratedAsync(ConsoleType.Wii, "0000000000000001", Region.Us, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(1, logger.WarnMessages.Count);
        }

        /// <summary>
        /// LoadCustomAsync with null path returns boundary failure.
        /// </summary>
        [TestMethod]
        public async Task LoadCustomAsync_NullPath_ReturnsBoundaryFailure()
        {
            var result = await sut.LoadCustomAsync(ConsoleType.Wii, null, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.CustomBaseRomInvalid, result.Errors[0].Code);
            Assert.AreEqual(0, catalog.LoadCustomCallCount);
        }

        /// <summary>
        /// LoadCustomAsync with empty path returns boundary failure.
        /// </summary>
        [TestMethod]
        public async Task LoadCustomAsync_EmptyPath_ReturnsBoundaryFailure()
        {
            var result = await sut.LoadCustomAsync(ConsoleType.Wii, "", CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.CustomBaseRomInvalid, result.Errors[0].Code);
            Assert.AreEqual(0, catalog.LoadCustomCallCount);
        }

        /// <summary>
        /// LoadCustomAsync with whitespace path returns boundary failure.
        /// </summary>
        [TestMethod]
        public async Task LoadCustomAsync_WhitespacePath_ReturnsBoundaryFailure()
        {
            var result = await sut.LoadCustomAsync(ConsoleType.Wii, "  ", CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.CustomBaseRomInvalid, result.Errors[0].Code);
            Assert.AreEqual(0, catalog.LoadCustomCallCount);
        }

        /// <summary>
        /// LoadCustomAsync delegates to catalog and logs success.
        /// </summary>
        [TestMethod]
        public async Task LoadCustomAsync_HappyPath_DelegatesAndLogsInfo()
        {
            var baseRom = new BaseRom("Custom", "CCCCCCCCCCCCCCCC", null, ConsoleType.Wii, true);
            catalog.NextLoadCustomResult = Result<BaseRom>.Success(baseRom);

            var result = await sut.LoadCustomAsync(ConsoleType.Wii, "C:\\custom.wua", CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(baseRom, result.Value);
            Assert.AreEqual(1, catalog.LoadCustomCallCount);
            Assert.AreEqual("C:\\custom.wua", catalog.LastLoadCustomPath);
            Assert.AreEqual(1, logger.InfoMessages.Count);
        }

        /// <summary>
        /// LoadCustomAsync propagates catalog failure.
        /// </summary>
        [TestMethod]
        public async Task LoadCustomAsync_CatalogFailure_PropagatesToCallerAndLogsWarning()
        {
            var error = new ApplicationError(ApplicationErrors.RomReadFailed, "Cannot read file.", null);
            catalog.NextLoadCustomResult = Result<BaseRom>.Failure(error);

            var result = await sut.LoadCustomAsync(ConsoleType.Wii, "C:\\invalid.wua", CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(1, logger.WarnMessages.Count);
        }
    }
}
