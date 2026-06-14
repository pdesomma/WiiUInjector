#nullable disable
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Application.Abstractions.Settings;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.Settings;
using WiiUInjector.Application.Tests.Fakes;
using WiiUInjector.Application.Tests.FakesB;
using FakeApplicationLogger = WiiUInjector.Application.Tests.Fakes.FakeApplicationLogger;

namespace WiiUInjector.Application.Tests.Services.Settings
{
    [TestClass]
    public class SettingsServiceTests
    {
        private SettingsService sut;
        private FakeApplicationSettingsStore store;
        private FakeApplicationLogger logger;

        [TestInitialize]
        public void Setup()
        {
            store = new FakeApplicationSettingsStore();
            logger = new FakeApplicationLogger();
            sut = new SettingsService(store, logger);
        }

        /// <summary>
        /// Ctor throws when store is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullStore_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SettingsService(null, logger));
        }

        /// <summary>
        /// Ctor throws when logger is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullLogger_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SettingsService(store, null));
        }

        /// <summary>
        /// LoadAsync happy path returns settings and logs info.
        /// </summary>
        [TestMethod]
        public async Task LoadAsync_HappyPath_ReturnsSettingsAndLogsInfo()
        {
            var settings = new ApplicationSettings
            {
                PathsSet = true,
                BasePath = "C:\\base",
                OutPath = "C:\\out"
            };
            store.NextLoadResult = Result<ApplicationSettings>.Success(settings);

            var result = await sut.LoadAsync(CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(settings, result.Value);
            Assert.AreEqual(1, store.LoadCallCount);
            Assert.AreEqual(1, logger.InfoMessages.Count);
            Assert.IsTrue(logger.InfoMessages[0].Contains("loaded"));
        }

        /// <summary>
        /// LoadAsync failure logs warning.
        /// </summary>
        [TestMethod]
        public async Task LoadAsync_StoreFails_LogsWarning()
        {
            var error = new ApplicationError(ApplicationErrors.SettingsLoadFailed, "Load failed.", null);
            store.NextLoadResult = Result<ApplicationSettings>.Failure(error);

            var result = await sut.LoadAsync(CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.SettingsLoadFailed, result.Errors[0].Code);
            Assert.AreEqual(1, logger.WarnMessages.Count);
        }

        /// <summary>
        /// SaveAsync with null settings returns boundary failure.
        /// </summary>
        [TestMethod]
        public async Task SaveAsync_NullSettings_ReturnsBoundaryFailure()
        {
            var result = await sut.SaveAsync(null, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.SettingsSaveFailed, result.Errors[0].Code);
            Assert.AreEqual(0, store.SaveCallCount);
        }

        /// <summary>
        /// SaveAsync happy path delegates and logs success.
        /// </summary>
        [TestMethod]
        public async Task SaveAsync_HappyPath_DelegatesAndLogsInfo()
        {
            var settings = new ApplicationSettings
            {
                PathsSet = true,
                BasePath = "C:\\base",
                OutPath = "C:\\out"
            };

            var result = await sut.SaveAsync(settings, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, store.SaveCallCount);
            Assert.AreEqual(settings, store.LastSaveSettings);
            Assert.AreEqual(1, logger.InfoMessages.Count);
            Assert.IsTrue(logger.InfoMessages[0].Contains("saved"));
        }

        /// <summary>
        /// SaveAsync failure logs warning.
        /// </summary>
        [TestMethod]
        public async Task SaveAsync_StoreFails_LogsWarning()
        {
            var settings = new ApplicationSettings();
            var error = new ApplicationError(ApplicationErrors.SettingsSaveFailed, "Write failed.", null);
            store.NextSaveResult = Result.Failure(error);

            var result = await sut.SaveAsync(settings, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(1, logger.WarnMessages.Count);
        }
    }
}
