#nullable disable
using WiiUInjector.Application.Abstractions.Operations;

namespace WiiUInjector.Application.Tests.Fakes
{
    /// <summary>
    /// Test double for IInjectionWorkspace providing canned paths and tracking Dispose calls.
    /// </summary>
    public sealed class FakeInjectionWorkspace : IInjectionWorkspace
    {
        /// <summary>
        /// Gets or sets the root directory path.
        /// </summary>
        public string RootDirectory { get; set; } = @"C:\fake\workspace";

        /// <summary>
        /// Gets the code directory path.
        /// </summary>
        public string CodeDirectory => RootDirectory + @"\code";

        /// <summary>
        /// Gets the content directory path.
        /// </summary>
        public string ContentDirectory => RootDirectory + @"\content";

        /// <summary>
        /// Gets the metadata directory path.
        /// </summary>
        public string MetaDirectory => RootDirectory + @"\meta";

        /// <summary>
        /// Gets the temporary directory path.
        /// </summary>
        public string TempDirectory => RootDirectory + @"\temp";

        /// <summary>
        /// Gets the number of times Dispose was called.
        /// </summary>
        public int DisposeCallCount { get; private set; }

        /// <summary>
        /// Increments the dispose call count.
        /// </summary>
        public void Dispose()
        {
            DisposeCallCount++;
        }
    }
}
