#nullable disable
using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Operations;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.Injections;

namespace WiiUInjector.Application.Tests.Fakes
{
    /// <summary>
    /// Test double for IWiiUPackager.
    /// </summary>
    public sealed class FakeWiiUPackager : IWiiUPackager
    {
        /// <summary>
        /// Gets or sets the result to return.
        /// </summary>
        public Result<InjectionOutcome> NextResult { get; set; } =
            Result<InjectionOutcome>.Success(
                new InjectionOutcome(@"C:\out\game.wup", new[] { "fake" }, TimeSpan.FromSeconds(1)));

        /// <summary>
        /// Gets the number of times PackAsync was called.
        /// </summary>
        public int CallCount { get; private set; }

        /// <summary>
        /// Gets the workspace passed to the last PackAsync call.
        /// </summary>
        public IInjectionWorkspace LastWorkspace { get; private set; }

        /// <summary>
        /// Gets the output spec passed to the last PackAsync call.
        /// </summary>
        public InjectionOutputSpec LastOutput { get; private set; }

        /// <summary>
        /// Gets or sets an exception to throw on the next call; null means don't throw.
        /// </summary>
        public Exception ThrowOnCall { get; set; }

        /// <summary>
        /// Packages the workspace asynchronously.
        /// </summary>
        public Task<Result<InjectionOutcome>> PackAsync(IInjectionWorkspace workspace, InjectionOutputSpec output, CancellationToken cancellationToken)
        {
            CallCount++;
            LastWorkspace = workspace;
            LastOutput = output;

            if (ThrowOnCall != null)
                throw ThrowOnCall;

            return Task.FromResult(NextResult);
        }
    }
}
