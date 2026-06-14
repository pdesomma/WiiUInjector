#nullable disable
using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Operations;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Tests.Fakes
{
    /// <summary>
    /// Test double for IWiiUMetadataEditor.
    /// </summary>
    public sealed class FakeWiiUMetadataEditor : IWiiUMetadataEditor
    {
        /// <summary>
        /// Gets or sets the result to return.
        /// </summary>
        public Result NextResult { get; set; } = Result.Success();

        /// <summary>
        /// Gets the number of times EditAsync was called.
        /// </summary>
        public int CallCount { get; private set; }

        /// <summary>
        /// Gets the metadata passed to the last EditAsync call.
        /// </summary>
        public WiiUMetadata LastMetadata { get; private set; }

        /// <summary>
        /// Gets the workspace passed to the last EditAsync call.
        /// </summary>
        public IInjectionWorkspace LastWorkspace { get; private set; }

        /// <summary>
        /// Gets or sets an exception to throw on the next call; null means don't throw.
        /// </summary>
        public Exception ThrowOnCall { get; set; }

        /// <summary>
        /// Edits the Wii U metadata asynchronously.
        /// </summary>
        public Task<Result> EditAsync(WiiUMetadata metadata, IInjectionWorkspace workspace, CancellationToken cancellationToken)
        {
            CallCount++;
            LastMetadata = metadata;
            LastWorkspace = workspace;

            if (ThrowOnCall != null)
                throw ThrowOnCall;

            return Task.FromResult(NextResult);
        }
    }
}
