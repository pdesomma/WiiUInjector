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
    /// Test double for IWiiUBootSoundWriter.
    /// </summary>
    public sealed class FakeWiiUBootSoundWriter : IWiiUBootSoundWriter
    {
        /// <summary>
        /// Gets or sets the result to return.
        /// </summary>
        public Result NextResult { get; set; } = Result.Success();

        /// <summary>
        /// Gets the number of times WriteAsync was called.
        /// </summary>
        public int CallCount { get; private set; }

        /// <summary>
        /// Gets the boot sound passed to the last WriteAsync call.
        /// </summary>
        public BootSound LastBootSound { get; private set; }

        /// <summary>
        /// Gets the workspace passed to the last WriteAsync call.
        /// </summary>
        public IInjectionWorkspace LastWorkspace { get; private set; }

        /// <summary>
        /// Gets or sets an exception to throw on the next call; null means don't throw.
        /// </summary>
        public Exception ThrowOnCall { get; set; }

        /// <summary>
        /// Writes the boot sound asynchronously.
        /// </summary>
        public Task<Result> WriteAsync(BootSound bootSound, IInjectionWorkspace workspace, CancellationToken cancellationToken)
        {
            CallCount++;
            LastBootSound = bootSound;
            LastWorkspace = workspace;

            if (ThrowOnCall != null)
                throw ThrowOnCall;

            return Task.FromResult(NextResult);
        }
    }
}
