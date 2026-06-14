#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Application.Abstractions.Logging;
using WiiUInjector.Application.Abstractions.Operations;
using WiiUInjector.Application.Abstractions.Operations.Injectors;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.Injections;
using WiiUInjector.Application.Tests.Fakes;
using WiiUInjector.Application.Tests.Fakes.Injectors;
using WiiUInjector.Domain;
using WiiUInjector.Domain.EmulatorOptions;

namespace WiiUInjector.Application.Tests.Services.Injections
{
    /// <summary>
    /// Tests for the InjectionPipeline orchestration.
    /// </summary>
    [TestClass]
    public class InjectionPipelineTests
    {
        private InjectionPipeline sut;
        private FakeInjectionWorkspaceFactory fakeFactory;
        private FakeBaseRomExtractor fakeExtractor;
        private FakeNesSnesRomInjector fakeNesSnesInjector;
        private FakeN64RomInjector fakeN64Injector;
        private FakeGbaRomInjector fakeGbaInjector;
        private FakeNdsRomInjector fakeNdsInjector;
        private FakeTurboGrafxRomInjector fakeTurboGrafxInjector;
        private FakeMsxRomInjector fakeMsxInjector;
        private FakeWiiIsoInjector fakeWiiInjector;
        private FakeGcnIsoInjector fakeGcnInjector;
        private FakeWiiUMetadataEditor fakeMetadataEditor;
        private FakeWiiUBootSoundWriter fakeBootSoundWriter;
        private FakeWiiUPackager fakePackager;
        private FakeApplicationLogger fakeLogger;

        [TestInitialize]
        public void Setup()
        {
            fakeFactory = new FakeInjectionWorkspaceFactory();
            fakeExtractor = new FakeBaseRomExtractor();
            fakeNesSnesInjector = new FakeNesSnesRomInjector();
            fakeN64Injector = new FakeN64RomInjector();
            fakeGbaInjector = new FakeGbaRomInjector();
            fakeNdsInjector = new FakeNdsRomInjector();
            fakeTurboGrafxInjector = new FakeTurboGrafxRomInjector();
            fakeMsxInjector = new FakeMsxRomInjector();
            fakeWiiInjector = new FakeWiiIsoInjector();
            fakeGcnInjector = new FakeGcnIsoInjector();
            fakeMetadataEditor = new FakeWiiUMetadataEditor();
            fakeBootSoundWriter = new FakeWiiUBootSoundWriter();
            fakePackager = new FakeWiiUPackager();
            fakeLogger = new FakeApplicationLogger();

            sut = new InjectionPipeline(
                fakeFactory,
                fakeExtractor,
                fakeNesSnesInjector,
                fakeN64Injector,
                fakeGbaInjector,
                fakeNdsInjector,
                fakeTurboGrafxInjector,
                fakeMsxInjector,
                fakeWiiInjector,
                fakeGcnInjector,
                fakeMetadataEditor,
                fakeBootSoundWriter,
                fakePackager,
                fakeLogger);
        }

        /// <summary>
        /// Ctor throws ArgumentNullException when any dependency is null.
        /// </summary>
        [DataTestMethod]
        [DataRow(0)]   // workspaceFactory
        [DataRow(1)]   // baseRomExtractor
        [DataRow(2)]   // nesSnesInjector
        [DataRow(3)]   // n64Injector
        [DataRow(4)]   // gbaInjector
        [DataRow(5)]   // ndsInjector
        [DataRow(6)]   // turboGrafxInjector
        [DataRow(7)]   // msxInjector
        [DataRow(8)]   // wiiInjector
        [DataRow(9)]   // gcnInjector
        [DataRow(10)]  // metadataEditor
        [DataRow(11)]  // bootSoundWriter
        [DataRow(12)]  // packager
        [DataRow(13)]  // logger
        public void Ctor_NullDep_AtPosition_Throws(int nullIndex)
        {
            var args = new object[]
            {
                fakeFactory,
                fakeExtractor,
                fakeNesSnesInjector,
                fakeN64Injector,
                fakeGbaInjector,
                fakeNdsInjector,
                fakeTurboGrafxInjector,
                fakeMsxInjector,
                fakeWiiInjector,
                fakeGcnInjector,
                fakeMetadataEditor,
                fakeBootSoundWriter,
                fakePackager,
                fakeLogger
            };

            args[nullIndex] = null;

            Assert.ThrowsException<ArgumentNullException>(() =>
                new InjectionPipeline(
                    (IInjectionWorkspaceFactory)args[0],
                    (IBaseRomExtractor)args[1],
                    (INesSnesRomInjector)args[2],
                    (IN64RomInjector)args[3],
                    (IGbaRomInjector)args[4],
                    (INdsRomInjector)args[5],
                    (ITurboGrafxRomInjector)args[6],
                    (IMsxRomInjector)args[7],
                    (IWiiIsoInjector)args[8],
                    (IGcnIsoInjector)args[9],
                    (IWiiUMetadataEditor)args[10],
                    (IWiiUBootSoundWriter)args[11],
                    (IWiiUPackager)args[12],
                    (IApplicationLogger)args[13]));
        }

        /// <summary>
        /// RunAsync returns WorkspaceCreateFailed when workspace factory fails.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_WorkspaceCreateFailure_ReturnsWorkspaceCreateFailed()
        {
            fakeFactory.NextResult = Result<IInjectionWorkspace>.Failure(
                new ApplicationError("WS_FAIL", "Workspace creation failed", null));

            var injection = BuildInjection(ConsoleType.N64);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            var result = await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.WorkspaceCreateFailed, result.Errors[0].Code);
            Assert.AreEqual(0, fakeExtractor.CallCount);
            Assert.AreEqual(0, fakeN64Injector.CallCount);
        }

        /// <summary>
        /// RunAsync returns BaseRomExtractFailed when extractor fails.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_ExtractorFailure_ReturnsBaseRomExtractFailed()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;
            fakeExtractor.NextResult = Result.Failure(
                new ApplicationError("EXTRACT_FAIL", "Extraction failed", null));

            var injection = BuildInjection(ConsoleType.N64);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            var result = await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.BaseRomExtractFailed, result.Errors[0].Code);
            Assert.AreEqual(0, fakeN64Injector.CallCount);
            Assert.AreEqual(1, workspace.DisposeCallCount);
        }

        /// <summary>
        /// RunAsync dispatches to NES injector for NES console.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_NesInjection_CallsNesSnesInjector()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;

            var injection = BuildInjection(ConsoleType.Nes);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.AreEqual(1, fakeNesSnesInjector.CallCount);
            Assert.AreEqual(0, fakeN64Injector.CallCount);
            Assert.AreEqual(0, fakeGbaInjector.CallCount);
            Assert.AreEqual(0, fakeNdsInjector.CallCount);
            Assert.AreEqual(0, fakeTurboGrafxInjector.CallCount);
            Assert.AreEqual(0, fakeMsxInjector.CallCount);
            Assert.AreEqual(0, fakeWiiInjector.CallCount);
            Assert.AreEqual(0, fakeGcnInjector.CallCount);
        }

        /// <summary>
        /// RunAsync dispatches to NES injector for SNES console.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_SnesInjection_CallsNesSnesInjector()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;

            var injection = BuildInjection(ConsoleType.Snes);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.AreEqual(1, fakeNesSnesInjector.CallCount);
            Assert.AreEqual(0, fakeN64Injector.CallCount);
            Assert.AreEqual(0, fakeGbaInjector.CallCount);
            Assert.AreEqual(0, fakeNdsInjector.CallCount);
            Assert.AreEqual(0, fakeTurboGrafxInjector.CallCount);
            Assert.AreEqual(0, fakeMsxInjector.CallCount);
            Assert.AreEqual(0, fakeWiiInjector.CallCount);
            Assert.AreEqual(0, fakeGcnInjector.CallCount);
        }

        /// <summary>
        /// RunAsync dispatches to N64 injector for N64 console.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_N64Injection_CallsN64Injector()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;

            var injection = BuildInjection(ConsoleType.N64);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.AreEqual(0, fakeNesSnesInjector.CallCount);
            Assert.AreEqual(1, fakeN64Injector.CallCount);
            Assert.AreEqual(0, fakeGbaInjector.CallCount);
            Assert.AreEqual(0, fakeNdsInjector.CallCount);
            Assert.AreEqual(0, fakeTurboGrafxInjector.CallCount);
            Assert.AreEqual(0, fakeMsxInjector.CallCount);
            Assert.AreEqual(0, fakeWiiInjector.CallCount);
            Assert.AreEqual(0, fakeGcnInjector.CallCount);
        }

        /// <summary>
        /// RunAsync dispatches to GBA injector for GBA console.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_GbaInjection_CallsGbaInjector()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;

            var injection = BuildInjection(ConsoleType.Gba);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.AreEqual(0, fakeNesSnesInjector.CallCount);
            Assert.AreEqual(0, fakeN64Injector.CallCount);
            Assert.AreEqual(1, fakeGbaInjector.CallCount);
            Assert.AreEqual(0, fakeNdsInjector.CallCount);
            Assert.AreEqual(0, fakeTurboGrafxInjector.CallCount);
            Assert.AreEqual(0, fakeMsxInjector.CallCount);
            Assert.AreEqual(0, fakeWiiInjector.CallCount);
            Assert.AreEqual(0, fakeGcnInjector.CallCount);
        }

        /// <summary>
        /// RunAsync dispatches to NDS injector for NDS console.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_NdsInjection_CallsNdsInjector()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;

            var injection = BuildInjection(ConsoleType.Nds);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.AreEqual(0, fakeNesSnesInjector.CallCount);
            Assert.AreEqual(0, fakeN64Injector.CallCount);
            Assert.AreEqual(0, fakeGbaInjector.CallCount);
            Assert.AreEqual(1, fakeNdsInjector.CallCount);
            Assert.AreEqual(0, fakeTurboGrafxInjector.CallCount);
            Assert.AreEqual(0, fakeMsxInjector.CallCount);
            Assert.AreEqual(0, fakeWiiInjector.CallCount);
            Assert.AreEqual(0, fakeGcnInjector.CallCount);
        }

        /// <summary>
        /// RunAsync dispatches to TurboGrafx injector for TurboGrafx console.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_TurboGrafxInjection_CallsTurboGrafxInjector()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;

            var injection = BuildInjection(ConsoleType.TurboGrafx);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.AreEqual(0, fakeNesSnesInjector.CallCount);
            Assert.AreEqual(0, fakeN64Injector.CallCount);
            Assert.AreEqual(0, fakeGbaInjector.CallCount);
            Assert.AreEqual(0, fakeNdsInjector.CallCount);
            Assert.AreEqual(1, fakeTurboGrafxInjector.CallCount);
            Assert.AreEqual(0, fakeMsxInjector.CallCount);
            Assert.AreEqual(0, fakeWiiInjector.CallCount);
            Assert.AreEqual(0, fakeGcnInjector.CallCount);
        }

        /// <summary>
        /// RunAsync dispatches to MSX injector for MSX console.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_MsxInjection_CallsMsxInjector()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;

            var injection = BuildInjection(ConsoleType.Msx);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.AreEqual(0, fakeNesSnesInjector.CallCount);
            Assert.AreEqual(0, fakeN64Injector.CallCount);
            Assert.AreEqual(0, fakeGbaInjector.CallCount);
            Assert.AreEqual(0, fakeNdsInjector.CallCount);
            Assert.AreEqual(0, fakeTurboGrafxInjector.CallCount);
            Assert.AreEqual(1, fakeMsxInjector.CallCount);
            Assert.AreEqual(0, fakeWiiInjector.CallCount);
            Assert.AreEqual(0, fakeGcnInjector.CallCount);
        }

        /// <summary>
        /// RunAsync dispatches to Wii injector for Wii console.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_WiiInjection_CallsWiiInjector()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;

            var injection = BuildInjection(ConsoleType.Wii);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.AreEqual(0, fakeNesSnesInjector.CallCount);
            Assert.AreEqual(0, fakeN64Injector.CallCount);
            Assert.AreEqual(0, fakeGbaInjector.CallCount);
            Assert.AreEqual(0, fakeNdsInjector.CallCount);
            Assert.AreEqual(0, fakeTurboGrafxInjector.CallCount);
            Assert.AreEqual(0, fakeMsxInjector.CallCount);
            Assert.AreEqual(1, fakeWiiInjector.CallCount);
            Assert.AreEqual(0, fakeGcnInjector.CallCount);
        }

        /// <summary>
        /// RunAsync dispatches to GCN injector for GCN console.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_GcnInjection_CallsGcnInjector()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;

            var injection = BuildInjection(ConsoleType.Gcn);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.AreEqual(0, fakeNesSnesInjector.CallCount);
            Assert.AreEqual(0, fakeN64Injector.CallCount);
            Assert.AreEqual(0, fakeGbaInjector.CallCount);
            Assert.AreEqual(0, fakeNdsInjector.CallCount);
            Assert.AreEqual(0, fakeTurboGrafxInjector.CallCount);
            Assert.AreEqual(0, fakeMsxInjector.CallCount);
            Assert.AreEqual(0, fakeWiiInjector.CallCount);
            Assert.AreEqual(1, fakeGcnInjector.CallCount);
        }

        /// <summary>
        /// RunAsync returns RomInjectionFailed when per-console injector fails.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_InjectorFailure_ReturnsRomInjectionFailed()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;
            fakeWiiInjector.NextResult = Result.Failure(
                new ApplicationError("INJ_FAIL", "boom", null));

            var injection = BuildInjection(ConsoleType.Wii);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            var result = await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.RomInjectionFailed, result.Errors[0].Code);
            Assert.IsTrue(result.Errors[0].Message.Contains("boom"));
            Assert.AreEqual(0, fakeMetadataEditor.CallCount);
            Assert.AreEqual(1, workspace.DisposeCallCount);
        }

        /// <summary>
        /// RunAsync returns MetadataEditFailed when metadata editor fails.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_MetadataEditFailure_ReturnsMetadataEditFailed()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;
            fakeMetadataEditor.NextResult = Result.Failure(
                new ApplicationError("META_FAIL", "Metadata edit failed", null));

            var injection = BuildInjection(ConsoleType.N64);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            var result = await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.MetadataEditFailed, result.Errors[0].Code);
            Assert.AreEqual(0, fakeBootSoundWriter.CallCount);
            Assert.AreEqual(1, workspace.DisposeCallCount);
        }

        /// <summary>
        /// RunAsync skips boot sound writer when boot sound is null.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_BootSoundNull_WriterNotCalled()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;

            var injection = BuildInjection(ConsoleType.N64);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.AreEqual(0, fakeBootSoundWriter.CallCount);
            Assert.AreEqual(1, fakePackager.CallCount);
        }

        /// <summary>
        /// RunAsync calls boot sound writer and packager when boot sound is non-null.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_BootSoundNonNull_WriterAndPackagerCalled()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;

            var bootSound = new BootSound(BootSoundFormat.Wav, new byte[] { 0x01, 0x02 });
            var baseInjection = BuildInjection(ConsoleType.N64);
            var injection = baseInjection.WithBootSound(bootSound);

            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.AreEqual(1, fakeBootSoundWriter.CallCount);
            Assert.AreEqual(1, fakePackager.CallCount);
        }

        /// <summary>
        /// RunAsync returns BootSoundWriteFailed when boot sound writer fails.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_BootSoundWriterFailure_ReturnsBootSoundWriteFailed()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;
            fakeBootSoundWriter.NextResult = Result.Failure(
                new ApplicationError("SOUND_FAIL", "Boot sound write failed", null));

            var bootSound = new BootSound(BootSoundFormat.Wav, new byte[] { 0x01, 0x02 });
            var baseInjection = BuildInjection(ConsoleType.N64);
            var injection = baseInjection.WithBootSound(bootSound);

            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            var result = await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.BootSoundWriteFailed, result.Errors[0].Code);
            Assert.AreEqual(0, fakePackager.CallCount);
            Assert.AreEqual(1, workspace.DisposeCallCount);
        }

        /// <summary>
        /// RunAsync returns PackagingFailed when packager fails.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_PackagerFailure_ReturnsPackagingFailed()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;
            fakePackager.NextResult = Result<InjectionOutcome>.Failure(
                new ApplicationError("PACK_FAIL", "Packaging failed", null));

            var injection = BuildInjection(ConsoleType.N64);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            var result = await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.PackagingFailed, result.Errors[0].Code);
            Assert.AreEqual(1, workspace.DisposeCallCount);
        }

        /// <summary>
        /// RunAsync returns success with packager outcome on happy path.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_HappyPath_ReturnsPackagerOutcome()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;

            var expectedOutcome = new InjectionOutcome(
                @"C:\out\game.wup",
                new[] { "warning1", "warning2" },
                TimeSpan.FromSeconds(42));
            fakePackager.NextResult = Result<InjectionOutcome>.Success(expectedOutcome);

            var injection = BuildInjection(ConsoleType.N64);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Packed, "key_source");
            var progress = new Progress<InjectionProgress>();

            var result = await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedOutcome.OutputPath, result.Value.OutputPath);
            Assert.AreEqual(2, result.Value.Warnings.Count);
            Assert.AreEqual(1, workspace.DisposeCallCount);
        }

        /// <summary>
        /// RunAsync reports progress at expected milestones and in monotonic order.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_HappyPath_ReportsMonotonicProgress()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;

            var progressReports = new List<InjectionProgress>();

            // Use a custom Progress<T> reporter that doesn't rely on SynchronizationContext
            var progress = new TestProgress<InjectionProgress>(p =>
            {
                progressReports.Add(p);
            });

            var injection = BuildInjection(ConsoleType.N64);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);

            await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.IsTrue(progressReports.Count > 0, "No progress reports captured");

            // Verify first report is 5%.
            Assert.AreEqual(5, progressReports.First().PercentComplete);

            // Verify last report is 100%.
            Assert.AreEqual(100, progressReports.Last().PercentComplete);

            // Verify monotonic (each report is >= previous).
            for (int i = 1; i < progressReports.Count; i++)
            {
                Assert.IsTrue(
                    progressReports[i].PercentComplete >= progressReports[i - 1].PercentComplete,
                    $"Progress not monotonic at index {i}: {progressReports[i].PercentComplete} < {progressReports[i - 1].PercentComplete}");
            }
        }

        /// <summary>
        /// Simple test implementation of IProgress that calls the handler directly without SynchronizationContext.
        /// </summary>
        private sealed class TestProgress<T> : IProgress<T>
        {
            private readonly Action<T> _handler;

            public TestProgress(Action<T> handler)
            {
                _handler = handler;
            }

            public void Report(T value)
            {
                _handler?.Invoke(value);
            }
        }

        /// <summary>
        /// RunAsync logs injection start and completion on happy path.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_HappyPath_LogsStartAndCompletion()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;

            var injection = BuildInjection(ConsoleType.N64);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.IsTrue(
                fakeLogger.InfoMessages.Any(m => m.Contains("Starting injection")),
                "No 'Starting injection' log message");
            Assert.IsTrue(
                fakeLogger.InfoMessages.Any(m => m.Contains("Injection complete")),
                "No 'Injection complete' log message");
        }

        /// <summary>
        /// RunAsync returns OperationCancelled when OperationCanceledException is thrown.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_OperationCancelledThrown_ReturnsOperationCancelled()
        {
            var workspace = new FakeInjectionWorkspace();
            fakeFactory.WorkspaceToReturn = workspace;
            fakeWiiInjector.ThrowOnCall = new OperationCanceledException();

            var injection = BuildInjection(ConsoleType.Wii);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            var result = await sut.RunAsync(injection, output, progress, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.OperationCancelled, result.Errors[0].Code);
            Assert.AreEqual(1, workspace.DisposeCallCount);
        }

        /// <summary>
        /// RunAsync returns PipelineFailed when injection argument is null.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_NullInjection_ReturnsPipelineFailed()
        {
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);
            var progress = new Progress<InjectionProgress>();

            var result = await sut.RunAsync(null, output, progress, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.PipelineFailed, result.Errors[0].Code);
        }

        /// <summary>
        /// RunAsync returns PipelineFailed when output argument is null.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_NullOutput_ReturnsPipelineFailed()
        {
            var injection = BuildInjection(ConsoleType.N64);
            var progress = new Progress<InjectionProgress>();

            var result = await sut.RunAsync(injection, null, progress, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.PipelineFailed, result.Errors[0].Code);
        }

        /// <summary>
        /// RunAsync returns PipelineFailed when progress argument is null.
        /// </summary>
        [TestMethod]
        public async Task RunAsync_NullProgress_ReturnsPipelineFailed()
        {
            var injection = BuildInjection(ConsoleType.N64);
            var output = new InjectionOutputSpec("C:\\out", InjectionOutputFormat.Loadiine, null);

            var result = await sut.RunAsync(injection, output, null, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(ApplicationErrors.PipelineFailed, result.Errors[0].Code);
        }

        // Helper methods

        /// <summary>
        /// Builds a fully valid injection for the specified console.
        /// </summary>
        private static Injection BuildInjection(ConsoleType console)
        {
            var baseRom = new BaseRom("base", "0005000010001234", Region.Us, console, false);
            var options = EmulatorOptionsDefaults.For(console);
            return new Injection(
                console,
                baseRom,
                TestData.AnyRom(1024),
                "rom.bin",
                TestData.AllImages(),
                TestData.BootScreen(),
                options);
        }
    }
}
