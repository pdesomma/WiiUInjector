#nullable disable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.IO;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Tests.Fakes
{
    /// <summary>
    /// Test double for IRomFileLoader with configurable success/failure behavior.
    /// </summary>
    public sealed class FakeRomFileLoader : IRomFileLoader
    {
        /// <summary>
        /// Configurable result to return from LoadAsync; defaults to success with a valid ROM.
        /// </summary>
        public Result<RomLoadResult> NextResult { get; set; }

        /// <summary>
        /// Number of times LoadAsync was called.
        /// </summary>
        public int CallCount { get; set; }

        /// <summary>
        /// The most recently requested file path.
        /// </summary>
        public string LastFilePath { get; set; }

        /// <summary>
        /// Initializes FakeRomFileLoader with a default successful result.
        /// </summary>
        public FakeRomFileLoader()
        {
            var defaultRom = new RomLoadResult(TestData.AnyRom(), "test.rom");
            NextResult = Result<RomLoadResult>.Success(defaultRom);
        }

        /// <summary>
        /// Returns the configured NextResult; records call count and file path.
        /// </summary>
        public Task<Result<RomLoadResult>> LoadAsync(string filePath, CancellationToken cancellationToken)
        {
            CallCount++;
            LastFilePath = filePath;
            return Task.FromResult(NextResult);
        }
    }
}
