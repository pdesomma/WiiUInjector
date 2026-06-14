#nullable disable
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Application.Abstractions.Updates;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.Updates;
using WiiUInjector.Application.Tests.Fakes;
using WiiUInjector.Application.Tests.FakesB;
using FakeApplicationLogger = WiiUInjector.Application.Tests.Fakes.FakeApplicationLogger;

namespace WiiUInjector.Application.Tests.Services.Updates
{
    [TestClass]
    public class UpdateServiceTests
    {
        private UpdateService sut;
        private FakeUpdateChecker checker;
        private FakeApplicationLogger logger;

        [TestInitialize]
        public void Setup()
        {
            checker = new FakeUpdateChecker();
            logger = new FakeApplicationLogger();
            sut = new UpdateService(checker, logger);
        }

        /// <summary>
        /// Ctor throws when checker is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullChecker_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UpdateService(null, logger));
        }

        /// <summary>
        /// Ctor throws when logger is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullLogger_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UpdateService(checker, null));
        }

        /// <summary>
        /// CheckForUpdatesAsync with null currentVersion returns UpdateCheckFailed error and does not call checker.
        /// </summary>
        [TestMethod]
        public async Task CheckForUpdatesAsync_NullCurrentVersion_ReturnsUpdateCheckFailedError()
        {
            var result = await sut.CheckForUpdatesAsync(null, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.UpdateCheckFailed, result.Errors[0].Code);
            Assert.AreEqual(0, checker.CheckCallCount);
        }

        /// <summary>
        /// CheckForUpdatesAsync with whitespace currentVersion returns UpdateCheckFailed error and does not call checker.
        /// </summary>
        [TestMethod]
        public async Task CheckForUpdatesAsync_WhitespaceCurrentVersion_ReturnsUpdateCheckFailedError()
        {
            var result = await sut.CheckForUpdatesAsync("  ", CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.UpdateCheckFailed, result.Errors[0].Code);
            Assert.AreEqual(0, checker.CheckCallCount);
        }

        /// <summary>
        /// CheckForUpdatesAsync happy path returns UpdateInfo and logs info.
        /// </summary>
        [TestMethod]
        public async Task CheckForUpdatesAsync_HappyPath_ReturnsUpdateInfoAndLogsInfo()
        {
            var updateInfo = new UpdateInfo("1.0", "2.0", "https://github.com/release", true, "Bug fixes");
            checker.NextCheckResult = Result<UpdateInfo>.Success(updateInfo);

            var result = await sut.CheckForUpdatesAsync("1.0", CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(updateInfo, result.Value);
            Assert.AreEqual(1, checker.CheckCallCount);
            Assert.AreEqual("1.0", checker.LastCheckVersion);
            Assert.AreEqual(1, logger.InfoMessages.Count);
            Assert.IsTrue(logger.InfoMessages[0].Contains("newer version available"));
        }

        /// <summary>
        /// CheckForUpdatesAsync when checker fails propagates the failure and logs warning.
        /// </summary>
        [TestMethod]
        public async Task CheckForUpdatesAsync_CheckerFails_PropagaturesFailureAndLogsWarning()
        {
            var error = new ApplicationError(ApplicationErrors.UpdateCheckFailed, "Network error.", null);
            checker.NextCheckResult = Result<UpdateInfo>.Failure(error);

            var result = await sut.CheckForUpdatesAsync("1.0", CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.UpdateCheckFailed, result.Errors[0].Code);
            Assert.AreEqual(1, logger.WarnMessages.Count);
        }
    }
}
