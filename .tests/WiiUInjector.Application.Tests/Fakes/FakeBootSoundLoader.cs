#nullable disable
using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.IO;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Tests.Fakes
{
    /// <summary>
    /// Test double for IBootSoundLoader with configurable success/failure behavior.
    /// </summary>
    public sealed class FakeBootSoundLoader : IBootSoundLoader
    {
        /// <summary>
        /// Configurable result to return from LoadAsync; defaults to a valid BootSound.
        /// </summary>
        public Result<BootSound> NextResult { get; set; }

        /// <summary>
        /// Number of times LoadAsync was called.
        /// </summary>
        public int CallCount { get; set; }

        /// <summary>
        /// The most recently requested file path.
        /// </summary>
        public string LastFilePath { get; set; }

        /// <summary>
        /// Initializes FakeBootSoundLoader with a default valid BootSound.
        /// </summary>
        public FakeBootSoundLoader()
        {
            var defaultSound = new BootSound(BootSoundFormat.Mp3, new byte[] { 0x01, 0x02 });
            NextResult = Result<BootSound>.Success(defaultSound);
        }

        /// <summary>
        /// Returns the configured NextResult; records call count and file path.
        /// </summary>
        public Task<Result<BootSound>> LoadAsync(string filePath, CancellationToken cancellationToken)
        {
            CallCount++;
            LastFilePath = filePath;
            return Task.FromResult(NextResult);
        }
    }
}
