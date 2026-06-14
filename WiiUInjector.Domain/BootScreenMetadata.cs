using System;

namespace WiiUInjector.Domain
{
    /// <summary>
    /// Immutable value object holding boot screen metadata (strings and numeric properties).
    /// </summary>
    public sealed class BootScreenMetadata
    {
        /// <summary>
        /// First line of the game name on the boot screen.
        /// </summary>
        public string NameLine1 { get; }

        /// <summary>
        /// Second line of the game name on the boot screen.
        /// </summary>
        public string NameLine2 { get; }

        /// <summary>
        /// Release year of the game; must be non-negative.
        /// </summary>
        public int ReleaseYear { get; }

        /// <summary>
        /// Number of players supported; must be non-negative.
        /// </summary>
        public int PlayerCount { get; }

        /// <summary>
        /// Full game name; used in menus and longer displays.
        /// </summary>
        public string LongName { get; }

        /// <summary>
        /// Constructs immutable boot screen metadata.
        /// </summary>
        /// <param name="nameLine1">First name line; cannot be null (empty is allowed).</param>
        /// <param name="nameLine2">Second name line; cannot be null (empty is allowed).</param>
        /// <param name="releaseYear">Release year; must be non-negative.</param>
        /// <param name="playerCount">Player count; must be non-negative.</param>
        /// <param name="longName">Long name; cannot be null (empty is allowed).</param>
        /// <exception cref="ArgumentNullException">Thrown if any string parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if releaseYear or playerCount is negative.</exception>
        public BootScreenMetadata(string nameLine1, string nameLine2, int releaseYear, int playerCount, string longName)
        {
            if (nameLine1 == null)
                throw new ArgumentNullException(nameof(nameLine1));

            if (nameLine2 == null)
                throw new ArgumentNullException(nameof(nameLine2));

            if (longName == null)
                throw new ArgumentNullException(nameof(longName));

            if (releaseYear < 0)
                throw new ArgumentOutOfRangeException(nameof(releaseYear), "Release year cannot be negative.");

            if (playerCount < 0)
                throw new ArgumentOutOfRangeException(nameof(playerCount), "Player count cannot be negative.");

            NameLine1 = nameLine1;
            NameLine2 = nameLine2;
            ReleaseYear = releaseYear;
            PlayerCount = playerCount;
            LongName = longName;
        }

        /// <summary>
        /// Structural equality comparing all fields using ordinal string comparison.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is BootScreenMetadata other))
                return false;

            return StringComparer.Ordinal.Equals(NameLine1, other.NameLine1)
                && StringComparer.Ordinal.Equals(NameLine2, other.NameLine2)
                && ReleaseYear == other.ReleaseYear
                && PlayerCount == other.PlayerCount
                && StringComparer.Ordinal.Equals(LongName, other.LongName);
        }

        /// <summary>
        /// Hash code combining all fields using ordinal string comparison.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + StringComparer.Ordinal.GetHashCode(NameLine1);
                hash = hash * 31 + StringComparer.Ordinal.GetHashCode(NameLine2);
                hash = hash * 31 + ReleaseYear.GetHashCode();
                hash = hash * 31 + PlayerCount.GetHashCode();
                hash = hash * 31 + StringComparer.Ordinal.GetHashCode(LongName);
                return hash;
            }
        }

        /// <summary>
        /// Structural equality operator.
        /// </summary>
        public static bool operator ==(BootScreenMetadata left, BootScreenMetadata right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Structural inequality operator.
        /// </summary>
        public static bool operator !=(BootScreenMetadata left, BootScreenMetadata right)
        {
            return !(left == right);
        }
    }
}
