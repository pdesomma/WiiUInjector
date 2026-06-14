#nullable disable
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.Cheats;
using WiiUInjector.Application.Tests.Fakes;
using WiiUInjector.Application.Tests.FakesB;

namespace WiiUInjector.Application.Tests.Services.Cheats
{
    /// <summary>
    /// Tests for CheatService.InjectCodehandler.
    /// </summary>
    [TestClass]
    public class CheatService_InjectCodehandlerTests
    {
        private CheatService sut;
        private FakeDolPatcher fakePatcher;
        private FakeApplicationLogger fakeLogger;

        [TestInitialize]
        public void Setup()
        {
            var fakeSource = new FakeGctSource();
            var fakeParser = new FakeGctParser();
            fakePatcher = new FakeDolPatcher();
            fakeLogger = new FakeApplicationLogger();

            sut = new CheatService(fakeSource, fakeParser, fakePatcher, fakeLogger);
        }

        /// <summary>
        /// InjectCodehandler returns failure when dolBytes is null.
        /// </summary>
        [TestMethod]
        public void InjectCodehandler_NullDolBytes_ReturnsFailure()
        {
            var codehandler = new byte[] { 0x00, 0x01 };

            var result = sut.InjectCodehandler(null, codehandler);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.DolPatchFailed, result.Errors[0].Code);
        }

        /// <summary>
        /// InjectCodehandler returns failure when codehandler is null.
        /// </summary>
        [TestMethod]
        public void InjectCodehandler_NullCodehandler_ReturnsFailure()
        {
            var dolBytes = new byte[] { 0x00, 0x01 };

            var result = sut.InjectCodehandler(dolBytes, null);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.DolPatchFailed, result.Errors[0].Code);
        }

        /// <summary>
        /// InjectCodehandler calls patcher and returns result.
        /// </summary>
        [TestMethod]
        public void InjectCodehandler_HappyPath_CallsPatcher()
        {
            var dolBytes = new byte[] { 0x00, 0x01 };
            var codehandler = new byte[] { 0xAA, 0xBB };
            var expectedResult = new byte[] { 0xFF, 0xEE };
            fakePatcher.InjectCodehandlerResult = expectedResult;

            var result = sut.InjectCodehandler(dolBytes, codehandler);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedResult, result.Value);
            Assert.AreEqual(1, fakePatcher.InjectCodehandlerCallCount);
        }

        /// <summary>
        /// InjectCodehandler returns failure when patcher throws exception.
        /// </summary>
        [TestMethod]
        public void InjectCodehandler_PatcherThrowsException_ReturnsFailure()
        {
            var dolBytes = new byte[] { 0x00, 0x01 };
            var codehandler = new byte[] { 0xAA, 0xBB };
            fakePatcher.InjectCodehandlerException = new InvalidOperationException("No free space");

            var result = sut.InjectCodehandler(dolBytes, codehandler);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.DolPatchFailed, result.Errors[0].Code);
            Assert.IsTrue(fakeLogger.ErrorMessages.Count >= 1);
        }

        /// <summary>
        /// InjectCodehandler logs success message.
        /// </summary>
        [TestMethod]
        public void InjectCodehandler_Success_LogsMessage()
        {
            var dolBytes = new byte[] { 0x00 };
            var codehandler = new byte[] { 0xAA };

            sut.InjectCodehandler(dolBytes, codehandler);

            Assert.IsTrue(fakeLogger.InfoMessages.Count >= 1);
        }
    }
}
