#nullable disable
using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Operations;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Tests.Fakes
{
    /// <summary>
    /// Test double for IBaseRomExtractor.
    /// </summary>
    public sealed class FakeBaseRomExtractor : IBaseRomExtractor
    {
        /// <summary>
        /// Gets or sets the result to return.
        /// </summary>
        public Result NextResult { get; set; } = Result.Success();

        /// <summary>
        /// Gets the number of times ExtractAsync was called.
        /// </summary>
        public int CallCount { get; private set; }

        /// <summary>
        /// Gets the BaseRom passed to the last ExtractAsync call.
        /// </summary>
        public BaseRom LastBaseRom { get; private set; }

        /// <summary>
        /// Gets the workspace passed to the last ExtractAsync call.
        /// </summary>
        public IInjectionWorkspace LastWorkspace { get; private set; }

        /// <summary>
        /// Gets or sets an exception to throw on the next call; null means don't throw.
        /// </summary>
        public Exception ThrowOnCall { get; set; }

        /// <summary>
        /// Extracts the base ROM asynchronously.
        /// </summary>
        public Task<Result> ExtractAsync(BaseRom baseRom, IInjectionWorkspace workspace, CancellationToken cancellationToken)
        {
            CallCount++;
            LastBaseRom = baseRom;
            LastWorkspace = workspace;

            if (ThrowOnCall != null)
                throw ThrowOnCall;

            return Task.FromResult(NextResult);
        }
    }
}
