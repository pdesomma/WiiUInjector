using System;

namespace WiiUInjector.Domain
{
    /// <summary>
    /// Entity representing a curated or user-supplied base ROM that an injection wraps.
    /// Identity is the composite (TitleId, Region).
    /// </summary>
    public sealed class BaseRom : IEquatable<BaseRom>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRom"/> class.
        /// </summary>
        /// <param name="name">Human-readable label.</param>
        /// <param name="titleId">Nintendo Title ID.</param>
        /// <param name="region">Region; can be null for region-less bases.</param>
        /// <param name="console">Console type this base targets.</param>
        /// <param name="isCustom">Whether this is a user-supplied base.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="name"/> or <paramref name="titleId"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="name"/> or <paramref name="titleId"/> is empty or whitespace.
        /// </exception>
        public BaseRom(string name, string titleId, Region? region, ConsoleType console, bool isCustom)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.", nameof(name));
            if (titleId == null) throw new ArgumentNullException(nameof(titleId));
            if (string.IsNullOrWhiteSpace(titleId)) throw new ArgumentException("TitleId cannot be empty.", nameof(titleId));

            Name = name;
            TitleId = titleId;
            Region = region;
            Console = console;
            IsCustom = isCustom;
        }

        /// <summary>
        /// Gets the console type this base targets.
        /// </summary>
        public ConsoleType Console { get; }
        /// <summary>
        /// Gets a value indicating whether this base is user-supplied (true)
        /// or from the curated catalog (false).
        /// </summary>
        public bool IsCustom { get; }
        /// <summary>
        /// Gets the human-readable label (e.g. "Super Smash Bros Melee EU").
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Gets the region; nullable for consoles that have region-less bases.
        /// Part of composite identity.
        /// </summary>
        public Region? Region { get; }
        /// <summary>
        /// Gets the Nintendo Title ID (16 hex chars typically).
        /// Part of composite identity.
        /// </summary>
        public string TitleId { get; }

        /// <summary>
        /// Determines whether the specified object is equal to the current base ROM.
        /// Equality is based on composite (TitleId, Region) only.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>
        /// true if the specified object is a BaseRom with the same TitleId and Region; otherwise, false.
        /// </returns>
        public override bool Equals(object obj) => obj is BaseRom other && Equals(other);
        /// <summary>
        /// Determines whether another rom is equal to the current base ROM.
        /// Equality is based on composite (TitleId, Region) only.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>
        /// true if the specified object has the same TitleId and Region; otherwise, false.
        /// </returns>
        public bool Equals(BaseRom other) => other != null && string.Equals(TitleId, other.TitleId, StringComparison.Ordinal) && Region == other.Region;
        /// <summary>
        /// Serves as the default hash function based on composite (TitleId, Region).
        /// </summary>
        /// <returns>A hash code for the current base ROM.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var h = StringComparer.Ordinal.GetHashCode(TitleId);
                h = h * 397 ^ Region.GetHashCode();
                return h;
            }
        }

        /// <summary>
        /// Determines whether two specified BaseRom instances are equal.
        /// </summary>
        /// <param name="left">The first BaseRom instance.</param>
        /// <param name="right">The second BaseRom instance.</param>
        /// <returns>true if the instances are equal; otherwise, false.</returns>
        public static bool operator ==(BaseRom left, BaseRom right) => ReferenceEquals(left, right) || (left?.Equals(right) ?? false);
        /// <summary>
        /// Determines whether two specified BaseRom instances are not equal.
        /// </summary>
        /// <param name="left">The first BaseRom instance.</param>
        /// <param name="right">The second BaseRom instance.</param>
        /// <returns>true if the instances are not equal; otherwise, false.</returns>
        public static bool operator !=(BaseRom left, BaseRom right) => !(left == right);
    }
}
