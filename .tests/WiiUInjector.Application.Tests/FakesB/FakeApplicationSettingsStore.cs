#nullable disable
using System;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Settings;
using WiiUInjector.Application.Common;

namespace WiiUInjector.Application.Tests.FakesB
{
    /// <summary>
    /// Test double for IApplicationSettingsStore that allows controlling responses.
    /// </summary>
    public sealed class FakeApplicationSettingsStore : IApplicationSettingsStore
    {
        /// <summary>
        /// Next result to return from LoadAsync.
        /// </summary>
        public Result<ApplicationSettings> NextLoadResult { get; set; }

        /// <summary>
        /// Next result to return from SaveAsync.
        /// </summary>
        public Result NextSaveResult { get; set; } = Result.Success();

        /// <summary>
        /// Number of times LoadAsync was called.
        /// </summary>
        public int LoadCallCount { get; set; }

        /// <summary>
        /// Number of times SaveAsync was called.
        /// </summary>
        public int SaveCallCount { get; set; }

        /// <summary>
        /// Last ApplicationSettings passed to SaveAsync.
        /// </summary>
        public ApplicationSettings LastSaveSettings { get; set; }

        /// <summary>
        /// Loads application settings asynchronously.
        /// </summary>
        public Task<Result<ApplicationSettings>> LoadAsync(CancellationToken cancellationToken)
        {
            LoadCallCount++;
            return Task.FromResult(NextLoadResult);
        }

        /// <summary>
        /// Saves the specified application settings asynchronously.
        /// </summary>
        public Task<Result> SaveAsync(ApplicationSettings settings, CancellationToken cancellationToken)
        {
            SaveCallCount++;
            LastSaveSettings = settings;
            return Task.FromResult(NextSaveResult);
        }
    }
}
