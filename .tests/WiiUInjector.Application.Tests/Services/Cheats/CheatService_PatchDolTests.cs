#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.Cheats;
using WiiUInjector.Application.Tests.Fakes;
using WiiUInjector.Application.Tests.FakesB;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Tests.Services.Cheats
{
    /// <summary>
    /// Tests for CheatService.PatchDol.
    /// </summary>
    [TestClass]
    public class CheatService_PatchDolTests
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
        /// PatchDol returns failure when dolBytes is null.
        /// </summary>
        [TestMethod]
        public void PatchDol_NullDolBytes_ReturnsFailure()
        {
            var codes = new List<GctCode> { new GctCode(0x80000000, 0x00000001) };

            var result = sut.PatchDol(null, codes);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.DolPatchFailed, result.Errors[0].Code);
        }

        /// <summary>
        /// PatchDol returns failure when codes list is null.
        /// </summary>
        [TestMethod]
        public void PatchDol_NullCodes_ReturnsFailure()
        {
            var dolBytes = new byte[] { 0x00, 0x01 };

            var result = sut.PatchDol(dolBytes, null);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.DolPatchFailed, result.Errors[0].Code);
        }

        /// <summary>
        /// PatchDol returns original bytes unchanged when codes list is empty.
        /// </summary>
        [TestMethod]
        public void PatchDol_EmptyCodesList_ReturnsSuccess()
        {
            var dolBytes = new byte[] { 0x00, 0x01, 0x02 };
            var codes = new List<GctCode>();

            var result = sut.PatchDol(dolBytes, codes);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(dolBytes, result.Value);
        }

        /// <summary>
        /// PatchDol calls patcher with correct arguments.
        /// </summary>
        [TestMethod]
        public void PatchDol_HappyPath_CallsPatcher()
        {
            var dolBytes = new byte[] { 0x00, 0x01 };
            var codes = new List<GctCode> { new GctCode(0x80000000, 0x00000001) };
            var expectedResult = new byte[] { 0xFF, 0xFF };
            fakePatcher.ApplyCodesResult = expectedResult;

            var result = sut.PatchDol(dolBytes, codes);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedResult, result.Value);
            Assert.AreEqual(1, fakePatcher.ApplyCodesCallCount);
        }

        /// <summary>
        /// PatchDol returns failure when patcher throws exception.
        /// </summary>
        [TestMethod]
        public void PatchDol_PatcherThrowsException_ReturnsFailure()
        {
            var dolBytes = new byte[] { 0x00, 0x01 };
            var codes = new List<GctCode> { new GctCode(0x80000000, 0x00000001) };
            fakePatcher.ApplyCodesException = new InvalidOperationException("Address out of range");

            var result = sut.PatchDol(dolBytes, codes);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.DolPatchFailed, result.Errors[0].Code);
            Assert.IsTrue(fakeLogger.ErrorMessages.Count >= 1);
        }

        /// <summary>
        /// PatchDol logs success message.
        /// </summary>
        [TestMethod]
        public void PatchDol_Success_LogsMessage()
        {
            var dolBytes = new byte[] { 0x00 };
            var codes = new List<GctCode> { new GctCode(0x80000000, 0x00000001) };

            sut.PatchDol(dolBytes, codes);

            Assert.IsTrue(fakeLogger.InfoMessages.Count >= 1);
        }
    }
}
