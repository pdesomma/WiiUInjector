#nullable disable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.TitleKeys;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Tests.FakesB
{
    /// <summary>
    /// Test double for ITitleKeyStore that allows controlling responses.
    /// </summary>
    public sealed class FakeTitleKeyStore : ITitleKeyStore
    {
        /// <summary>
        /// Next result to return from GetAsync.
        /// </summary>
        public Result<TitleKey> NextGetResult { get; set; }

        /// <summary>
        /// Next result to return from ListAsync.
        /// </summary>
        public Result<IReadOnlyList<TitleKey>> NextListResult { get; set; } = Result<IReadOnlyList<TitleKey>>.Success(new List<TitleKey>());

        /// <summary>
        /// Next result to return from SaveAsync.
        /// </summary>
        public Result NextSaveResult { get; set; } = Result.Success();

        /// <summary>
        /// Next result to return from DeleteAsync.
        /// </summary>
        public Result NextDeleteResult { get; set; } = Result.Success();

        /// <summary>
        /// Number of times GetAsync was called.
        /// </summary>
        public int GetCallCount { get; set; }

        /// <summary>
        /// Last console passed to GetAsync.
        /// </summary>
        public ConsoleType? LastGetConsole { get; set; }

        /// <summary>
        /// Last titleId passed to GetAsync.
        /// </summary>
        public string LastGetTitleId { get; set; }

        /// <summary>
        /// Last region passed to GetAsync.
        /// </summary>
        public Region? LastGetRegion { get; set; }

        /// <summary>
        /// Number of times ListAsync was called.
        /// </summary>
        public int ListCallCount { get; set; }

        /// <summary>
        /// Last console passed to ListAsync.
        /// </summary>
        public ConsoleType? LastListConsole { get; set; }

        /// <summary>
        /// Number of times SaveAsync was called.
        /// </summary>
        public int SaveCallCount { get; set; }

        /// <summary>
        /// Last TitleKey passed to SaveAsync.
        /// </summary>
        public TitleKey LastSaveKey { get; set; }

        /// <summary>
        /// Number of times DeleteAsync was called.
        /// </summary>
        public int DeleteCallCount { get; set; }

        /// <summary>
        /// Last console passed to DeleteAsync.
        /// </summary>
        public ConsoleType? LastDeleteConsole { get; set; }

        /// <summary>
        /// Last titleId passed to DeleteAsync.
        /// </summary>
        public string LastDeleteTitleId { get; set; }

        /// <summary>
        /// Last region passed to DeleteAsync.
        /// </summary>
        public Region? LastDeleteRegion { get; set; }

        /// <summary>
        /// Retrieves a title key by console, title ID, and optional region.
        /// </summary>
        public Task<Result<TitleKey>> GetAsync(ConsoleType console, string titleId, Region? region, CancellationToken cancellationToken)
        {
            GetCallCount++;
            LastGetConsole = console;
            LastGetTitleId = titleId;
            LastGetRegion = region;
            return Task.FromResult(NextGetResult);
        }

        /// <summary>
        /// Lists all title keys for a specified console.
        /// </summary>
        public Task<Result<IReadOnlyList<TitleKey>>> ListAsync(ConsoleType console, CancellationToken cancellationToken)
        {
            ListCallCount++;
            LastListConsole = console;
            return Task.FromResult(NextListResult);
        }

        /// <summary>
        /// Persists a title key (insert or update).
        /// </summary>
        public Task<Result> SaveAsync(TitleKey key, CancellationToken cancellationToken)
        {
            SaveCallCount++;
            LastSaveKey = key;
            return Task.FromResult(NextSaveResult);
        }

        /// <summary>
        /// Deletes a title key by console, title ID, and optional region.
        /// </summary>
        public Task<Result> DeleteAsync(ConsoleType console, string titleId, Region? region, CancellationToken cancellationToken)
        {
            DeleteCallCount++;
            LastDeleteConsole = console;
            LastDeleteTitleId = titleId;
            LastDeleteRegion = region;
            return Task.FromResult(NextDeleteResult);
        }
    }
}
