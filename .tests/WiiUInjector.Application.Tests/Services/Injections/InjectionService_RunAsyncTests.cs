#nullable disable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.Injections;
using WiiUInjector.Application.Tests.Fakes;
using WiiUInjector.Application.Tests.FakesB;
using WiiUInjector.Domain;
using WiiUInjector.Domain.Services;

namespace WiiUInjector.Application.Tests.Services.Injections
{
    /// <summary>
    /// Tests for InjectionService.RunAsync pipeline execution.
    /// </summary>
    [TestClass]
    public class InjectionService_RunAsyncTests
    {
        private InjectionService sut;
        private FakeInjectionValidator fakeValidator;
        private FakeInjectionPipeline fakePipeline;
        private FakeApplicationLogger fakeLogger;

        [TestInitialize]
        public void Setup()
        {
            var fakeRomLoader = new FakeRomFileLoader();
            var fakeImageLoader = new FakeGameImageLoader();
            var fakeSoundLoader = new FakeBootSoundLoader();
            var fakeBaseRomCatalog = new FakesB.FakeBaseRomCatalog();
            fakeBaseRomCatalog.NextFindResult = Result<BaseRom>.Success(TestData.AnyBaseRom());
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
        /// RunAsync returns failure when command is null.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_NullCommand_ReturnsFailure()
        {
            // Don't need to set fakeToolProvisioner since null command returns immediately
            var progress = new Progress<InjectionProgress>();
            var result = await sut.RunAsync(null, progress, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.DomainInvariantViolation, result.Errors[0].Code);
        }

        /// <summary>
        /// RunAsync returns failure when progress is null.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_NullProgress_ReturnsFailure()
        {
            // Don't need to set fakeToolProvisioner since null progress returns immediately
            var injection = TestData.AnyN64Injection();
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var cmd = new RunInjectionCommand(injection, output);

            var result = await sut.RunAsync(cmd, null, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.DomainInvariantViolation, result.Errors[0].Code);
        }

        /// <summary>
        /// RunAsync does not call pipeline when validator fails.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_ValidatorFails_PipelineNotCalled()
        {
            var errors = new List<ValidationError>
            {
                new ValidationError("TEST_ERROR", "Test error", null)
            };
            fakeValidator.NextResult = new ValidationResult(errors);

            var injection = TestData.AnyN64Injection();
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var cmd = new RunInjectionCommand(injection, output);
            var progress = new Progress<InjectionProgress>();

            var result = await sut.RunAsync(cmd, progress, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(0, fakePipeline.CallCount);
        }

        /// <summary>
        /// RunAsync calls pipeline and returns its outcome on success.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_HappyPath_CallsPipelineAndReturnsOutcome()
        {
            var expectedOutcome = new InjectionOutcome("package.wua", new List<string>(), TimeSpan.FromSeconds(1));
            fakePipeline.NextResult = Result<InjectionOutcome>.Success(expectedOutcome);

            var injection = TestData.AnyN64Injection();
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var cmd = new RunInjectionCommand(injection, output);
            var progress = new Progress<InjectionProgress>();

            var result = await sut.RunAsync(cmd, progress, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedOutcome.OutputPath, result.Value.OutputPath);
            Assert.AreEqual(1, fakePipeline.CallCount);
        }

        /// <summary>
        /// RunAsync logs Info message on successful injection start.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_Success_LogsInfoMessage()
        {
            var injection = TestData.AnyN64Injection();
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var cmd = new RunInjectionCommand(injection, output);
            var progress = new Progress<InjectionProgress>();

            await sut.RunAsync(cmd, progress, CancellationToken.None);

            Assert.IsTrue(fakeLogger.InfoMessages.Count >= 1);
        }

        /// <summary>
        /// RunAsync returns failure when pipeline throws OperationCanceledException.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_PipelineThrowsOperationCancelled_ReturnsCancelledError()
        {
            fakePipeline.NextException = new OperationCanceledException("User cancelled");

            var injection = TestData.AnyN64Injection();
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var cmd = new RunInjectionCommand(injection, output);
            var progress = new Progress<InjectionProgress>();

            var result = await sut.RunAsync(cmd, progress, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.OperationCancelled, result.Errors[0].Code);
        }

        /// <summary>
        /// RunAsync returns failure when pipeline throws generic exception and logs error.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_PipelineThrowsException_ReturnsFailureAndLogsError()
        {
            var testException = new InvalidOperationException("Pipeline failed");
            fakePipeline.NextException = testException;

            var injection = TestData.AnyN64Injection();
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var cmd = new RunInjectionCommand(injection, output);
            var progress = new Progress<InjectionProgress>();

            var result = await sut.RunAsync(cmd, progress, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.PipelineFailed, result.Errors[0].Code);
            Assert.IsTrue(fakeLogger.ErrorMessages.Count >= 1);
        }
    }
}
