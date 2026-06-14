using System;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Services.Injections
{
    /// <summary>
    /// Sealed value object representing the user's choice of base ROM — either a curated catalog entry or a custom file.
    /// </summary>
    public sealed class BaseRomSelection
    {
        /// <summary>
        /// The kind of base ROM selection (Curated or Custom).
        /// </summary>
        public BaseRomSelectionKind Kind { get; }

        /// <summary>
        /// Nintendo Title ID for curated selections; null for custom selections.
        /// </summary>
        public string TitleId { get; }

        /// <summary>
        /// Region for curated selections; null for custom selections.
        /// </summary>
        public Region? Region { get; }

        /// <summary>
        /// File path for custom selections; null for curated selections.
        /// </summary>
        public string CustomPath { get; }

        /// <summary>
        /// Initializes a new BaseRomSelection (private; use factory methods).
        /// </summary>
        /// <param name="kind">The kind of selection.</param>
        /// <param name="titleId">Title ID for curated selections; null otherwise.</param>
        /// <param name="region">Region for curated selections; null otherwise.</param>
        /// <param name="customPath">File path for custom selections; null otherwise.</param>
        private BaseRomSelection(BaseRomSelectionKind kind, string titleId, Region? region, string customPath)
        {
            Kind = kind;
            TitleId = titleId;
            Region = region;
            CustomPath = customPath;
        }

        /// <summary>
        /// Factory method to create a curated base ROM selection.
        /// </summary>
        /// <param name="titleId">Nintendo Title ID; must not be null or whitespace.</param>
        /// <param name="region">Optional region; may be null for region-less bases.</param>
        /// <returns>A new BaseRomSelection representing a curated entry.</returns>
        /// <exception cref="ArgumentNullException">Thrown if titleId is null.</exception>
        /// <exception cref="ArgumentException">Thrown if titleId is empty or whitespace.</exception>
        public static BaseRomSelection Curated(string titleId, Region? region)
        {
            if (titleId == null)
                throw new ArgumentNullException(nameof(titleId));

            if (string.IsNullOrWhiteSpace(titleId))
                throw new ArgumentException("TitleId cannot be empty.", nameof(titleId));

            return new BaseRomSelection(BaseRomSelectionKind.Curated, titleId, region, null);
        }

        /// <summary>
        /// Factory method to create a custom base ROM selection.
        /// </summary>
        /// <param name="customPath">Absolute or relative path to the custom base ROM file; must not be null or whitespace.</param>
        /// <returns>A new BaseRomSelection representing a custom file.</returns>
        /// <exception cref="ArgumentNullException">Thrown if customPath is null.</exception>
        /// <exception cref="ArgumentException">Thrown if customPath is empty or whitespace.</exception>
        public static BaseRomSelection Custom(string customPath)
        {
            if (customPath == null)
                throw new ArgumentNullException(nameof(customPath));

            if (string.IsNullOrWhiteSpace(customPath))
                throw new ArgumentException("CustomPath cannot be empty.", nameof(customPath));

            return new BaseRomSelection(BaseRomSelectionKind.Custom, null, null, customPath);
        }
    }
}
