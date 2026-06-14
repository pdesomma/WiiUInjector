using System;

namespace WiiUInjector.Application.Abstractions.Operations
{
    /// <summary>
    /// Immutable value object representing options for NFS encoding via nfs2iso2nfs.
    /// Captures all command-line flags that control the encoding behavior for Wii and GameCube ISOs.
    /// </summary>
    public sealed class NfsEncodingOptions : IEquatable<NfsEncodingOptions>
    {
        /// <summary>
        /// Gets a value indicating whether to use homebrew mode (-homebrew flag).
        /// </summary>
        public bool Homebrew { get; }

        /// <summary>
        /// Gets a value indicating whether to enable passthrough mode (-passthrough flag).
        /// </summary>
        public bool Passthrough { get; }

        /// <summary>
        /// Gets a value indicating whether to apply the LR patch (-lrpatch flag).
        /// Used for Wii injections to enable shoulder button functionality.
        /// </summary>
        public bool LrPatch { get; }

        /// <summary>
        /// Gets a value indicating whether to force Classic Controller mode (-instantcc flag).
        /// </summary>
        public bool ForceCC { get; }

        /// <summary>
        /// Gets a value indicating whether to disable horizontal IR pointer (-horizontal flag).
        /// </summary>
        public bool DisableHorizontalIR { get; }

        /// <summary>
        /// Gets the encryption key as a hexadecimal string; null when not encrypting.
        /// Used with the -enc flag if provided.
        /// </summary>
        public string EncryptionKeyHex { get; }

        /// <summary>
        /// Constructs an immutable NfsEncodingOptions with the specified flags.
        /// </summary>
        /// <param name="homebrew">Whether to use homebrew mode.</param>
        /// <param name="passthrough">Whether to enable passthrough mode.</param>
        /// <param name="lrPatch">Whether to apply the LR patch.</param>
        /// <param name="forceCC">Whether to force Classic Controller mode.</param>
        /// <param name="disableHorizontalIR">Whether to disable horizontal IR.</param>
        /// <param name="encryptionKeyHex">Encryption key as hex string; may be null. If provided, must be a valid hex string.</param>
        /// <exception cref="ArgumentException">Thrown if encryptionKeyHex is not null and contains invalid hex characters.</exception>
        public NfsEncodingOptions(
            bool homebrew = false,
            bool passthrough = false,
            bool lrPatch = false,
            bool forceCC = false,
            bool disableHorizontalIR = false,
            string encryptionKeyHex = null)
        {
            if (encryptionKeyHex != null)
            {
                if (!IsValidHexString(encryptionKeyHex))
                    throw new ArgumentException("Encryption key must be a valid hexadecimal string.", nameof(encryptionKeyHex));
            }

            Homebrew = homebrew;
            Passthrough = passthrough;
            LrPatch = lrPatch;
            ForceCC = forceCC;
            DisableHorizontalIR = disableHorizontalIR;
            EncryptionKeyHex = encryptionKeyHex;
        }

        /// <summary>
        /// Structural equality comparing all properties.
        /// </summary>
        public override bool Equals(object obj) => obj is NfsEncodingOptions other && Equals(other);

        /// <summary>
        /// Structural equality comparing all properties.
        /// </summary>
        public bool Equals(NfsEncodingOptions other)
        {
            if (other == null)
                return false;

            return Homebrew == other.Homebrew
                && Passthrough == other.Passthrough
                && LrPatch == other.LrPatch
                && ForceCC == other.ForceCC
                && DisableHorizontalIR == other.DisableHorizontalIR
                && string.Equals(EncryptionKeyHex, other.EncryptionKeyHex, StringComparison.Ordinal);
        }

        /// <summary>
        /// Hash code combining all properties.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Homebrew.GetHashCode();
                hash = hash * 31 + Passthrough.GetHashCode();
                hash = hash * 31 + LrPatch.GetHashCode();
                hash = hash * 31 + ForceCC.GetHashCode();
                hash = hash * 31 + DisableHorizontalIR.GetHashCode();
                hash = hash * 31 + (EncryptionKeyHex?.GetHashCode() ?? 0);
                return hash;
            }
        }

        /// <summary>
        /// Structural equality operator.
        /// </summary>
        public static bool operator ==(NfsEncodingOptions left, NfsEncodingOptions right)
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
        public static bool operator !=(NfsEncodingOptions left, NfsEncodingOptions right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Validates whether a string is a valid hexadecimal representation.
        /// </summary>
        /// <param name="hexString">The string to validate.</param>
        /// <returns>true if the string contains only valid hex characters (0-9, a-f, A-F); otherwise, false.</returns>
        private static bool IsValidHexString(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
                return false;

            if (hexString.Length % 2 != 0)
                return false;

            foreach (char c in hexString)
            {
                if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
                    return false;
            }

            return true;
        }
    }
}
