#nullable disable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.TitleKeys;
using WiiUInjector.Application.Tests.Fakes;
using WiiUInjector.Application.Tests.FakesB;
using FakeApplicationLogger = WiiUInjector.Application.Tests.Fakes.FakeApplicationLogger;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Tests.Services.TitleKeys
{
    [TestClass]
    public class TitleKeyServiceTests
    {
        private TitleKeyService sut;
        private FakeTitleKeyStore store;
        private FakeApplicationLogger logger;

        [TestInitialize]
        public void Setup()
        {
            store = new FakeTitleKeyStore();
            logger = new FakeApplicationLogger();
            sut = new TitleKeyService(store, logger);
        }

        /// <summary>
        /// Ctor throws when store is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullStore_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TitleKeyService(null, logger));
        }

        /// <summary>
        /// Ctor throws when logger is null.
        /// </summary>
        [TestMethod]
        public void Ctor_NullLogger_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TitleKeyService(store, null));
        }

        /// <summary>
        /// GetAsync returns not-found error and logs at Info level.
        /// </summary>
        [TestMethod]
        public async Task GetAsync_NotFound_LogsAtInfoLevel()
        {
            var error = new ApplicationError(ApplicationErrors.TitleKeyNotFound, "Key not found.", null);
            store.NextGetResult = Result<TitleKey>.Failure(error);

            var result = await sut.GetAsync(ConsoleType.Wii, "0000000000000001", Region.Us, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.TitleKeyNotFound, result.Errors[0].Code);
            Assert.AreEqual(1, logger.InfoMessages.Count);
            Assert.AreEqual(0, logger.WarnMessages.Count);
        }

        /// <summary>
        /// GetAsync other failure logs at Warn level.
        /// </summary>
        [TestMethod]
        public async Task GetAsync_OtherFailure_LogsAtWarnLevel()
        {
            var error = new ApplicationError(ApplicationErrors.TitleKeyStoreFailed, "Store error.", null);
            store.NextGetResult = Result<TitleKey>.Failure(error);

            var result = await sut.GetAsync(ConsoleType.Wii, "0000000000000001", Region.Us, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(1, logger.WarnMessages.Count);
            Assert.AreEqual(0, logger.InfoMessages.Count);
        }

        /// <summary>
        /// GetAsync happy path returns TitleKey and logs at Info level.
        /// </summary>
        [TestMethod]
        public async Task GetAsync_HappyPath_ReturnsKeyAndLogsInfo()
        {
            var key = new TitleKey(ConsoleType.Wii, "0000000000000001", Region.Us, new byte[] { 0x01, 0x02 });
            store.NextGetResult = Result<TitleKey>.Success(key);

            var result = await sut.GetAsync(ConsoleType.Wii, "0000000000000001", Region.Us, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(key, result.Value);
            Assert.AreEqual(1, logger.InfoMessages.Count);
            Assert.AreEqual(0, logger.WarnMessages.Count);
        }

        /// <summary>
        /// ListAsync happy path returns list.
        /// </summary>
        [TestMethod]
        public async Task ListAsync_HappyPath_ReturnsList()
        {
            var key1 = new TitleKey(ConsoleType.Wii, "0000000000000001", Region.Us, new byte[] { 0x01 });
            var key2 = new TitleKey(ConsoleType.Wii, "0000000000000002", Region.Eu, new byte[] { 0x02 });
            var keys = new List<TitleKey> { key1, key2 };

            store.NextListResult = Result<IReadOnlyList<TitleKey>>.Success(keys);

            var result = await sut.ListAsync(ConsoleType.Wii, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(2, result.Value.Count);
            Assert.AreEqual(1, logger.InfoMessages.Count);
        }

        /// <summary>
        /// SaveAsync with null key returns boundary failure.
        /// </summary>
        [TestMethod]
        public async Task SaveAsync_NullKey_ReturnsBoundaryFailure()
        {
            var result = await sut.SaveAsync(null, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.CustomBaseRomInvalid, result.Errors[0].Code);
            Assert.AreEqual(0, store.SaveCallCount);
        }

        /// <summary>
        /// SaveAsync happy path delegates and logs success.
        /// </summary>
        [TestMethod]
        public async Task SaveAsync_HappyPath_DelegatesAndLogsInfo()
        {
            var key = new TitleKey(ConsoleType.Wii, "0000000000000001", Region.Us, new byte[] { 0x01 });

            var result = await sut.SaveAsync(key, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, store.SaveCallCount);
            Assert.AreEqual(key, store.LastSaveKey);
            Assert.AreEqual(1, logger.InfoMessages.Count);
        }

        /// <summary>
        /// SaveAsync failure logs warning.
        /// </summary>
        [TestMethod]
        public async Task SaveAsync_StoreFails_LogsWarning()
        {
            var key = new TitleKey(ConsoleType.Wii, "0000000000000001", Region.Us, new byte[] { 0x01 });
            var error = new ApplicationError(ApplicationErrors.TitleKeyStoreFailed, "Write failed.", null);
            store.NextSaveResult = Result.Failure(error);

            var result = await sut.SaveAsync(key, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(1, logger.WarnMessages.Count);
        }

        /// <summary>
        /// DeleteAsync with empty titleId returns boundary failure.
        /// </summary>
        [TestMethod]
        public async Task DeleteAsync_EmptyTitleId_ReturnsBoundaryFailure()
        {
            var result = await sut.DeleteAsync(ConsoleType.Wii, "", Region.Us, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.CustomBaseRomInvalid, result.Errors[0].Code);
            Assert.AreEqual(0, store.DeleteCallCount);
        }

        /// <summary>
        /// DeleteAsync with null titleId returns boundary failure.
        /// </summary>
        [TestMethod]
        public async Task DeleteAsync_NullTitleId_ReturnsBoundaryFailure()
        {
            var result = await sut.DeleteAsync(ConsoleType.Wii, null, Region.Us, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.CustomBaseRomInvalid, result.Errors[0].Code);
            Assert.AreEqual(0, store.DeleteCallCount);
        }

        /// <summary>
        /// DeleteAsync happy path delegates and logs success.
        /// </summary>
        [TestMethod]
        public async Task DeleteAsync_HappyPath_DelegatesAndLogsInfo()
        {
            var result = await sut.DeleteAsync(ConsoleType.Wii, "0000000000000001", Region.Us, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, store.DeleteCallCount);
            Assert.AreEqual("0000000000000001", store.LastDeleteTitleId);
            Assert.AreEqual(1, logger.InfoMessages.Count);
        }
    }
}
