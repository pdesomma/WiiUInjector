#nullable disable
using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.IO;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Tests.Fakes
{
    /// <summary>
    /// Test double for IGctSource with configurable binary/text format and data.
    /// </summary>
    public sealed class FakeGctSource : IGctSource
    {
        /// <summary>
        /// Configurable result for ReadBytesAsync; defaults to valid bytes.
        /// </summary>
        public Result<byte[]> BytesResult { get; set; }

        /// <summary>
        /// Configurable result for ReadTextAsync; defaults to valid text.
        /// </summary>
        public Result<string> TextResult { get; set; }

        /// <summary>
        /// Whether IsBinaryGctFile should return true; defaults to true.
        /// </summary>
        public bool IsBinary { get; set; } = true;

        /// <summary>
        /// Number of times ReadBytesAsync was called.
        /// </summary>
        public int ReadBytesCallCount { get; set; }

        /// <summary>
        /// Number of times ReadTextAsync was called.
        /// </summary>
        public int ReadTextCallCount { get; set; }

        /// <summary>
        /// Most recently requested file path.
        /// </summary>
        public string LastFilePath { get; set; }

        /// <summary>
        /// Initializes FakeGctSource with default results.
        /// </summary>
        public FakeGctSource()
        {
            BytesResult = Result<byte[]>.Success(new byte[] { 0x00, 0x01 });
            TextResult = Result<string>.Success("AAAAAAAA VVVVVVVV");
        }

        /// <summary>
        /// Returns BytesResult; records the call.
        /// </summary>
        public Task<Result<byte[]>> ReadBytesAsync(string filePath, CancellationToken cancellationToken)
        {
            ReadBytesCallCount++;
            LastFilePath = filePath;
            return Task.FromResult(BytesResult);
        }

        /// <summary>
        /// Returns TextResult; records the call.
        /// </summary>
        public Task<Result<string>> ReadTextAsync(string filePath, CancellationToken cancellationToken)
        {
            ReadTextCallCount++;
            LastFilePath = filePath;
            return Task.FromResult(TextResult);
        }

        /// <summary>
        /// Returns the configured IsBinary flag.
        /// </summary>
        public bool IsBinaryGctFile(string filePath)
        {
            return IsBinary;
        }
    }
}
