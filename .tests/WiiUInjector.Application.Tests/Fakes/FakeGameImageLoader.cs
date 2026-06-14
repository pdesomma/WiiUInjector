#nullable disable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Abstractions.IO;
using WiiUInjector.Application.Common;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Tests.Fakes
{
    /// <summary>
    /// Test double for IGameImageLoader with per-slot configurable success/failure behavior.
    /// </summary>
    public sealed class FakeGameImageLoader : IGameImageLoader
    {
        /// <summary>
        /// Results to return per ImageSlot; populate before calling LoadAsync.
        /// </summary>
        public Dictionary<ImageSlot, Result<GameImage>> Results { get; } = new Dictionary<ImageSlot, Result<GameImage>>
        {
            { ImageSlot.Icon, Result<GameImage>.Success(TestData.Icon128()) },
            { ImageSlot.Drc, Result<GameImage>.Success(TestData.Drc854()) },
            { ImageSlot.Tv, Result<GameImage>.Success(TestData.Tv1280()) },
            { ImageSlot.Logo, Result<GameImage>.Success(TestData.Logo170()) }
        };

        /// <summary>
        /// Number of times LoadAsync was called.
        /// </summary>
        public int CallCount { get; set; }

        /// <summary>
        /// Most recently requested slots; allows inspection of which slots were loaded.
        /// </summary>
        public List<ImageSlot> RequestedSlots { get; } = new List<ImageSlot>();

        /// <summary>
        /// Returns the configured result for the requested slot; records the call.
        /// </summary>
        public Task<Result<GameImage>> LoadAsync(string filePath, ImageSlot slot, CancellationToken cancellationToken)
        {
            CallCount++;
            RequestedSlots.Add(slot);

            if (Results.ContainsKey(slot))
                return Task.FromResult(Results[slot]);

            return Task.FromResult(
                Result<GameImage>.Failure(
                    new ApplicationError(ApplicationErrors.ImageNotFound, $"No result configured for slot {slot}.")));
        }
    }
}
