#nullable disable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Application.Abstractions.Environment;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.Environment;
using WiiUInjector.Application.Tests.Fakes;
using WiiUInjector.Application.Tests.FakesB;
using FakeApplicationLogger = WiiUInjector.Application.Tests.Fakes.FakeApplicationLogger;

namespace WiiUInjector.Application.Tests.Services.Environment
{
    [TestClass]
    public class EnvironmentServiceTests
    {
        private EnvironmentService sut;
        private FakeEnvironmentInspector inspector;
        private FakeApplicationLogger logger;

        [TestInitialize]
        public void Setup()
        {
            inspector = new FakeEnvironmentInspector();
            logger = new FakeApplicationLogger();
            sut = new EnvironmentService(inspector, logger);
        }

        /// <summary>
        /// Ctor throws when inspector is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullInspector_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new EnvironmentService(null, logger));
        }

        /// <summary>
        /// Ctor throws when logger is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullLogger_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new EnvironmentService(inspector, null));
        }

        /// <summary>
        /// InspectAsync happy path returns EnvironmentReport and logs info including resolution and Wine/OneDrive flags.
        /// </summary>
        [TestMethod]
        public async Task InspectAsync_HappyPath_ReturnsReportAndLogsInfo()
        {
            var notes = new List<string> { "Note 1", "Note 2" };
            var report = new EnvironmentReport(
                isRunningFromOneDrive: false,
                isDuplicateInstance: false,
                screenMeetsMinimumResolution: true,
                screenWidth: 1920,
                screenHeight: 1080,
                isRunningUnderWineOrSimilar: false,
                notes: notes);

            inspector.NextInspectResult = Result<EnvironmentReport>.Success(report);

            var result = await sut.InspectAsync(CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(report, result.Value);
            Assert.AreEqual(1, inspector.InspectCallCount);
            Assert.AreEqual(1, logger.InfoMessages.Count);
            var logMsg = logger.InfoMessages[0];
            Assert.IsTrue(logMsg.Contains("1920"));
            Assert.IsTrue(logMsg.Contains("1080"));
            Assert.IsTrue(logMsg.Contains("OneDrive"));
            Assert.IsTrue(logMsg.Contains("Wine"));
        }

        /// <summary>
        /// InspectAsync failure logs warning.
        /// </summary>
        [TestMethod]
        public async Task InspectAsync_InspectorFails_LogsWarning()
        {
            var error = new ApplicationError(ApplicationErrors.EnvironmentInspectionFailed, "Inspection failed.", null);
            inspector.NextInspectResult = Result<EnvironmentReport>.Failure(error);

            var result = await sut.InspectAsync(CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.EnvironmentInspectionFailed, result.Errors[0].Code);
            Assert.AreEqual(1, logger.WarnMessages.Count);
            Assert.AreEqual(0, logger.InfoMessages.Count);
        }
    }
}
