#nullable disable
using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Environment;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Tests.FakesB
{
    /// <summary>
    /// Test double for IEnvironmentInspector that allows controlling responses.
    /// </summary>
    public sealed class FakeEnvironmentInspector : IEnvironmentInspector
    {
        /// <summary>
        /// Next result to return from InspectAsync.
        /// </summary>
        public Result<EnvironmentReport> NextInspectResult { get; set; }

        /// <summary>
        /// Number of times InspectAsync was called.
        /// </summary>
        public int InspectCallCount { get; set; }

        /// <summary>
        /// Inspects the system environment asynchronously and returns a report.
        /// </summary>
        public Task<Result<EnvironmentReport>> InspectAsync(CancellationToken cancellationToken)
        {
            InspectCallCount++;
            return Task.FromResult(NextInspectResult);
        }
    }
}
