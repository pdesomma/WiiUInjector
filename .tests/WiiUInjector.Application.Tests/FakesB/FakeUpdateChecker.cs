#nullable disable
using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Updates;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Tests.FakesB
{
    /// <summary>
    /// Test double for IUpdateChecker that allows controlling responses.
    /// </summary>
    public sealed class FakeUpdateChecker : IUpdateChecker
    {
        /// <summary>
        /// Next result to return from CheckAsync.
        /// </summary>
        public Result<UpdateInfo> NextCheckResult { get; set; }

        /// <summary>
        /// Number of times CheckAsync was called.
        /// </summary>
        public int CheckCallCount { get; set; }

        /// <summary>
        /// Last currentVersion passed to CheckAsync.
        /// </summary>
        public string LastCheckVersion { get; set; }

        /// <summary>
        /// Checks for available updates asynchronously.
        /// </summary>
        public Task<Result<UpdateInfo>> CheckAsync(string currentVersion, CancellationToken cancellationToken)
        {
            CheckCallCount++;
            LastCheckVersion = currentVersion;
            return Task.FromResult(NextCheckResult);
        }
    }
}
