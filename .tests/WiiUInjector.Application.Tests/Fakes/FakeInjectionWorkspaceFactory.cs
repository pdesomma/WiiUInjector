#nullable disable
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Operations;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Tests.Fakes
{
    /// <summary>
    /// Test double for IInjectionWorkspaceFactory.
    /// </summary>
    public sealed class FakeInjectionWorkspaceFactory : IInjectionWorkspaceFactory
    {
        /// <summary>
        /// Gets or sets the workspace instance to return on success.
        /// </summary>
        public FakeInjectionWorkspace WorkspaceToReturn { get; set; } = new FakeInjectionWorkspace();

        /// <summary>
        /// Gets or sets the next result to return; if null, returns Success(WorkspaceToReturn).
        /// </summary>
        public Result<IInjectionWorkspace> NextResult { get; set; }

        /// <summary>
        /// Gets the number of times CreateAsync was called.
        /// </summary>
        public int CallCount { get; private set; }

        /// <summary>
        /// Gets the console type passed to the last CreateAsync call.
        /// </summary>
        public ConsoleType LastConsole { get; private set; }

        /// <summary>
        /// Creates a new workspace asynchronously.
        /// </summary>
        public Task<Result<IInjectionWorkspace>> CreateAsync(ConsoleType console, CancellationToken cancellationToken)
        {
            CallCount++;
            LastConsole = console;

            if (NextResult != null)
                return Task.FromResult(NextResult);

            return Task.FromResult(Result<IInjectionWorkspace>.Success(WorkspaceToReturn));
        }
    }
}
