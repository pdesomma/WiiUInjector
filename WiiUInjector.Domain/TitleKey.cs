using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WiiUInjector.Domain
{
    /// <summary>
    /// Immutable value object mapping a base ROM identity to its decryption key bytes.
    /// </summary>
    public sealed class TitleKey
    {
        /// <summary>
        /// Console type this key applies to.
        /// </summary>
        public ConsoleType Console { get; }

        /// <summary>
        /// Base ROM title identifier; cannot be null or empty.
        /// </summary>
        public string TitleId { get; }

        /// <summary>
        /// Region of the base ROM (nullable; some bases are region-less).
        /// </summary>
        public Region? Region { get; }

        /// <summary>
        /// Decryption key bytes; read-only wrapper over an internal defensive copy.
        /// </summary>
        public IReadOnlyList<byte> KeyBytes { get; }

        /// <summary>
        /// Constructs an immutable TitleKey with defensive copying of the byte array.
        /// </summary>
        /// <param name="console">Console type for this key.</param>
        /// <param name="titleId">Base ROM identifier; cannot be null or empty.</param>
        /// <param name="region">Console region; may be null for region-less bases.</param>
        /// <param name="keyBytes">Decryption key bytes; cannot be null or empty.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="titleId"/> or <paramref name="keyBytes"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="titleId"/> is empty or only whitespace, or if <paramref name="keyBytes"/> is empty.</exception>
        public TitleKey(ConsoleType console, string titleId, Region? region, byte[] keyBytes)
        {
            if (titleId == null)
                throw new ArgumentNullException(nameof(titleId));

            if (string.IsNullOrWhiteSpace(titleId))
                throw new ArgumentException("TitleId cannot be empty.", nameof(titleId));

            if (keyBytes == null)
                throw new ArgumentNullException(nameof(keyBytes));

            if (keyBytes.Length == 0)
                throw new ArgumentException("Key bytes cannot be empty.", nameof(keyBytes));

            var copy = new byte[keyBytes.Length];
            Array.Copy(keyBytes, copy, keyBytes.Length);

            Console = console;
            TitleId = titleId;
            Region = region;
            KeyBytes = new ReadOnlyCollection<byte>(copy);
        }

        /// <summary>
        /// Structural equality comparing console, title ID (ordinal), region, and key bytes.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is TitleKey other))
                return false;

            if (Console != other.Console || Region != other.Region)
                return false;

            if (!StringComparer.Ordinal.Equals(TitleId, other.TitleId))
                return false;

            if (KeyBytes.Count != other.KeyBytes.Count)
                return false;

            for (int i = 0; i < KeyBytes.Count; i++)
            {
                if (KeyBytes[i] != other.KeyBytes[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Hash code combining console, title ID (ordinal), region, and key byte length.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Console.GetHashCode();
                hash = hash * 31 + StringComparer.Ordinal.GetHashCode(TitleId);
                hash = hash * 31 + Region.GetHashCode();
                hash = hash * 31 + KeyBytes.Count.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Structural equality operator.
        /// </summary>
        public static bool operator ==(TitleKey left, TitleKey right)
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
        public static bool operator !=(TitleKey left, TitleKey right)
        {
            return !(left == right);
        }
    }
}
