#nullable disable
using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Operations;
using WiiUInjector.Application.Abstractions.Operations.Injectors;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Tests.Fakes.Injectors
{
    /// <summary>
    /// Test double for IWiiIsoInjector.
    /// </summary>
    public sealed class FakeWiiIsoInjector : IWiiIsoInjector
    {
        /// <summary>
        /// Gets or sets the result to return.
        /// </summary>
        public Result NextResult { get; set; } = Result.Success();

        /// <summary>
        /// Gets the number of times InjectAsync was called.
        /// </summary>
        public int CallCount { get; private set; }

        /// <summary>
        /// Gets the injection passed to the last InjectAsync call.
        /// </summary>
        public Injection LastInjection { get; private set; }

        /// <summary>
        /// Gets the workspace passed to the last InjectAsync call.
        /// </summary>
        public IInjectionWorkspace LastWorkspace { get; private set; }

        /// <summary>
        /// Gets or sets an exception to throw on the next call; null means don't throw.
        /// </summary>
        public Exception ThrowOnCall { get; set; }

        /// <summary>
        /// Injects a Wii ISO asynchronously.
        /// </summary>
        public Task<Result> InjectAsync(Injection injection, IInjectionWorkspace workspace, CancellationToken cancellationToken)
        {
            CallCount++;
            LastInjection = injection;
            LastWorkspace = workspace;

            if (ThrowOnCall != null)
                throw ThrowOnCall;

            return Task.FromResult(NextResult);
        }
    }
}
