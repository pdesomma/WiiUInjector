using System;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Abstractions.Operations
{
    /// <summary>
    /// Immutable value object capturing metadata fields to be written into Wii U package XML files (meta.xml, app.xml).
    /// </summary>
    public sealed class WiiUMetadata : IEquatable<WiiUMetadata>
    {
        /// <summary>
        /// Gets the Nintendo Title ID for the package (non-empty).
        /// </summary>
        public string TitleId { get; }

        /// <summary>
        /// Gets the product code for the package (non-empty).
        /// </summary>
        public string ProductCode { get; }

        /// <summary>
        /// Gets the group ID for the package (non-empty).
        /// </summary>
        public string GroupId { get; }

        /// <summary>
        /// Gets the boot screen metadata.
        /// </summary>
        public BootScreenMetadata BootScreen { get; }

        /// <summary>
        /// Gets a value indicating whether to use the DRC (GamePad) screen.
        /// Corresponds to the legacy drc_use="65537" toggle.
        /// </summary>
        public bool UseDrcScreen { get; }

        /// <summary>
        /// Constructs immutable Wii U metadata.
        /// </summary>
        /// <param name="titleId">The Title ID; must not be null or whitespace.</param>
        /// <param name="productCode">The product code; must not be null or whitespace.</param>
        /// <param name="groupId">The group ID; must not be null or whitespace.</param>
        /// <param name="bootScreen">The boot screen metadata; must not be null.</param>
        /// <param name="useDrcScreen">Whether to use the DRC screen.</param>
        /// <exception cref="ArgumentNullException">Thrown if titleId, productCode, groupId, or bootScreen is null.</exception>
        /// <exception cref="ArgumentException">Thrown if titleId, productCode, or groupId is empty or whitespace.</exception>
        public WiiUMetadata(
            string titleId,
            string productCode,
            string groupId,
            BootScreenMetadata bootScreen,
            bool useDrcScreen)
        {
            if (titleId == null)
                throw new ArgumentNullException(nameof(titleId));
            if (string.IsNullOrWhiteSpace(titleId))
                throw new ArgumentException("Title ID cannot be empty or whitespace.", nameof(titleId));

            if (productCode == null)
                throw new ArgumentNullException(nameof(productCode));
            if (string.IsNullOrWhiteSpace(productCode))
                throw new ArgumentException("Product code cannot be empty or whitespace.", nameof(productCode));

            if (groupId == null)
                throw new ArgumentNullException(nameof(groupId));
            if (string.IsNullOrWhiteSpace(groupId))
                throw new ArgumentException("Group ID cannot be empty or whitespace.", nameof(groupId));

            if (bootScreen == null)
                throw new ArgumentNullException(nameof(bootScreen));

            TitleId = titleId;
            ProductCode = productCode;
            GroupId = groupId;
            BootScreen = bootScreen;
            UseDrcScreen = useDrcScreen;
        }

        /// <summary>
        /// Structural equality comparing all properties.
        /// </summary>
        public override bool Equals(object obj) => obj is WiiUMetadata other && Equals(other);

        /// <summary>
        /// Structural equality comparing all properties using ordinal string comparison.
        /// </summary>
        public bool Equals(WiiUMetadata other)
        {
            if (other == null)
                return false;

            return string.Equals(TitleId, other.TitleId, StringComparison.Ordinal)
                && string.Equals(ProductCode, other.ProductCode, StringComparison.Ordinal)
                && string.Equals(GroupId, other.GroupId, StringComparison.Ordinal)
                && BootScreen == other.BootScreen
                && UseDrcScreen == other.UseDrcScreen;
        }

        /// <summary>
        /// Hash code combining all properties.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + StringComparer.Ordinal.GetHashCode(TitleId);
                hash = hash * 31 + StringComparer.Ordinal.GetHashCode(ProductCode);
                hash = hash * 31 + StringComparer.Ordinal.GetHashCode(GroupId);
                hash = hash * 31 + BootScreen.GetHashCode();
                hash = hash * 31 + UseDrcScreen.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Structural equality operator.
        /// </summary>
        public static bool operator ==(WiiUMetadata left, WiiUMetadata right)
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
        public static bool operator !=(WiiUMetadata left, WiiUMetadata right)
        {
            return !(left == right);
        }
    }
}
