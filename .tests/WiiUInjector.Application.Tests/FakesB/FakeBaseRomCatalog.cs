#nullable disable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.Catalog;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Tests.FakesB
{
    /// <summary>
    /// Test double for IBaseRomCatalog that allows controlling responses.
    /// </summary>
    public sealed class FakeBaseRomCatalog : IBaseRomCatalog
    {
        /// <summary>
        /// Next result to return from ListCuratedAsync.
        /// </summary>
        public Result<IReadOnlyList<BaseRom>> NextListResult { get; set; } = Result<IReadOnlyList<BaseRom>>.Success(new List<BaseRom>());

        /// <summary>
        /// Next result to return from FindCuratedAsync.
        /// </summary>
        public Result<BaseRom> NextFindResult { get; set; }

        /// <summary>
        /// Next result to return from LoadCustomAsync.
        /// </summary>
        public Result<BaseRom> NextLoadCustomResult { get; set; }

        /// <summary>
        /// Number of times ListCuratedAsync was called.
        /// </summary>
        public int ListCallCount { get; set; }

        /// <summary>
        /// Last console passed to ListCuratedAsync.
        /// </summary>
        public ConsoleType? LastListConsole { get; set; }

        /// <summary>
        /// Number of times FindCuratedAsync was called.
        /// </summary>
        public int FindCallCount { get; set; }

        /// <summary>
        /// Last console passed to FindCuratedAsync.
        /// </summary>
        public ConsoleType? LastFindConsole { get; set; }

        /// <summary>
        /// Last titleId passed to FindCuratedAsync.
        /// </summary>
        public string LastFindTitleId { get; set; }

        /// <summary>
        /// Last region passed to FindCuratedAsync.
        /// </summary>
        public Region? LastFindRegion { get; set; }

        /// <summary>
        /// Number of times LoadCustomAsync was called.
        /// </summary>
        public int LoadCustomCallCount { get; set; }

        /// <summary>
        /// Last console passed to LoadCustomAsync.
        /// </summary>
        public ConsoleType? LastLoadCustomConsole { get; set; }

        /// <summary>
        /// Last path passed to LoadCustomAsync.
        /// </summary>
        public string LastLoadCustomPath { get; set; }

        /// <summary>
        /// Lists all curated base ROMs for the specified console.
        /// </summary>
        public Task<Result<IReadOnlyList<BaseRom>>> ListCuratedAsync(ConsoleType console, CancellationToken cancellationToken)
        {
            ListCallCount++;
            LastListConsole = console;
            return Task.FromResult(NextListResult);
        }

        /// <summary>
        /// Finds a curated base ROM by console, title ID, and optional region.
        /// </summary>
        public Task<Result<BaseRom>> FindCuratedAsync(ConsoleType console, string titleId, Region? region, CancellationToken cancellationToken)
        {
            FindCallCount++;
            LastFindConsole = console;
            LastFindTitleId = titleId;
            LastFindRegion = region;
            return Task.FromResult(NextFindResult);
        }

        /// <summary>
        /// Loads a custom base ROM from a local file path.
        /// </summary>
        public Task<Result<BaseRom>> LoadCustomAsync(ConsoleType console, string customPath, CancellationToken cancellationToken)
        {
            LoadCustomCallCount++;
            LastLoadCustomConsole = console;
            LastLoadCustomPath = customPath;
            return Task.FromResult(NextLoadCustomResult);
        }
    }
}
