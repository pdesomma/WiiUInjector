#nullable disable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.Cheats;
using WiiUInjector.Application.Tests.Fakes;
using WiiUInjector.Application.Tests.FakesB;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Tests.Services.Cheats
{
    /// <summary>
    /// Tests for CheatService.LoadCodesAsync.
    /// </summary>
    [TestClass]
    public class CheatService_LoadCodesTests
    {
        private CheatService sut;
        private FakeGctSource fakeSource;
        private FakeGctParser fakeParser;
        private FakeApplicationLogger fakeLogger;

        [TestInitialize]
        public void Setup()
        {
            fakeSource = new FakeGctSource();
            fakeParser = new FakeGctParser();
            fakeLogger = new FakeApplicationLogger();

            sut = new CheatService(fakeSource, fakeParser, new FakeDolPatcher(), fakeLogger);
        }

        /// <summary>
        /// LoadCodesAsync returns failure when filePath is null.
        /// </summary>
        [TestMethod]
        public async Task LoadCodesAsync_NullFilePath_ReturnsFailure()
        {
            var result = await sut.LoadCodesAsync(null, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.GctParseFailed, result.Errors[0].Code);
        }

        /// <summary>
        /// LoadCodesAsync returns failure when filePath is empty.
        /// </summary>
        [TestMethod]
        public async Task LoadCodesAsync_EmptyFilePath_ReturnsFailure()
        {
            var result = await sut.LoadCodesAsync(string.Empty, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.GctParseFailed, result.Errors[0].Code);
        }

        /// <summary>
        /// LoadCodesAsync calls ReadBytesAsync when file is binary format.
        /// </summary>
        [TestMethod]
        public async Task LoadCodesAsync_BinaryFormat_CallsReadBytesAsync()
        {
            fakeSource.IsBinary = true;
            var codes = new List<GctCode> { new GctCode(0x80000000, 0x00000001) };
            fakeParser.NextBinaryCodes = codes;

            var result = await sut.LoadCodesAsync("codes.gct", CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, fakeSource.ReadBytesCallCount);
            Assert.AreEqual(0, fakeSource.ReadTextCallCount);
            Assert.AreEqual(1, fakeParser.ParseBinaryCallCount);
        }

        /// <summary>
        /// LoadCodesAsync calls ReadTextAsync when file is text format.
        /// </summary>
        [TestMethod]
        public async Task LoadCodesAsync_TextFormat_CallsReadTextAsync()
        {
            fakeSource.IsBinary = false;
            var codes = new List<GctCode> { new GctCode(0x80000000, 0x00000001) };
            fakeParser.NextTextCodes = codes;

            var result = await sut.LoadCodesAsync("codes.txt", CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, fakeSource.ReadTextCallCount);
            Assert.AreEqual(0, fakeSource.ReadBytesCallCount);
            Assert.AreEqual(1, fakeParser.ParseTextCallCount);
        }

        /// <summary>
        /// LoadCodesAsync returns failure when ReadBytesAsync fails.
        /// </summary>
        [TestMethod]
        public async Task LoadCodesAsync_ReadBytesFails_ReturnsFailure()
        {
            fakeSource.IsBinary = true;
            fakeSource.BytesResult = Result<byte[]>.Failure(
                new ApplicationError(ApplicationErrors.RomNotFound, "File not found."));

            var result = await sut.LoadCodesAsync("codes.gct", CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
        }

        /// <summary>
        /// LoadCodesAsync returns failure when ReadTextAsync fails.
        /// </summary>
        [TestMethod]
        public async Task LoadCodesAsync_ReadTextFails_ReturnsFailure()
        {
            fakeSource.IsBinary = false;
            fakeSource.TextResult = Result<string>.Failure(
                new ApplicationError(ApplicationErrors.RomNotFound, "File not found."));

            var result = await sut.LoadCodesAsync("codes.txt", CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
        }

        /// <summary>
        /// LoadCodesAsync returns failure when parser throws exception.
        /// </summary>
        [TestMethod]
        public async Task LoadCodesAsync_ParserThrowsException_ReturnsFailure()
        {
            fakeSource.IsBinary = true;
            fakeParser.BinaryException = new InvalidOperationException("Bad format");

            var result = await sut.LoadCodesAsync("codes.gct", CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.GctParseFailed, result.Errors[0].Code);
            Assert.IsTrue(fakeLogger.ErrorMessages.Count >= 1);
        }

        /// <summary>
        /// LoadCodesAsync returns success with parsed codes.
        /// </summary>
        [TestMethod]
        public async Task LoadCodesAsync_HappyPath_ReturnsSuccessWithCodes()
        {
            var expectedCodes = new List<GctCode>
            {
                new GctCode(0x80000000, 0x00000001),
                new GctCode(0x80000004, 0x00000002)
            };
            fakeSource.IsBinary = true;
            fakeParser.NextBinaryCodes = expectedCodes;

            var result = await sut.LoadCodesAsync("codes.gct", CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Value.Count);
            Assert.IsTrue(fakeLogger.InfoMessages.Count >= 1);
        }
    }
}
