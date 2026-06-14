using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WiiUInjector.Domain
{
    /// <summary>
    /// Immutable value object holding boot sound bytes and format metadata.
    /// </summary>
    public sealed class BootSound
    {
        /// <summary>
        /// Audio format (e.g., MP3, WAV, or native Wii U boot sound).
        /// </summary>
        public BootSoundFormat Format { get; }

        /// <summary>
        /// Raw audio bytes; read-only wrapper over an internal defensive copy.
        /// </summary>
        public IReadOnlyList<byte> Bytes { get; }

        /// <summary>
        /// Constructs an immutable BootSound with defensive copying of the byte array.
        /// </summary>
        /// <param name="format">The audio format of the sound.</param>
        /// <param name="bytes">Raw audio bytes; cannot be null or empty.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="bytes"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="bytes"/> is empty.</exception>
        public BootSound(BootSoundFormat format, byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length == 0)
                throw new ArgumentException("Audio bytes cannot be empty.", nameof(bytes));

            var copy = new byte[bytes.Length];
            Array.Copy(bytes, copy, bytes.Length);

            Format = format;
            Bytes = new ReadOnlyCollection<byte>(copy);
        }

        /// <summary>
        /// Structural equality comparing format and bytes.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is BootSound other))
                return false;

            if (Format != other.Format)
                return false;

            if (Bytes.Count != other.Bytes.Count)
                return false;

            for (int i = 0; i < Bytes.Count; i++)
            {
                if (Bytes[i] != other.Bytes[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Hash code combining format and byte array length.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Format.GetHashCode();
                hash = hash * 31 + Bytes.Count.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Structural equality operator.
        /// </summary>
        public static bool operator ==(BootSound left, BootSound right)
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
        public static bool operator !=(BootSound left, BootSound right)
        {
            return !(left == right);
        }
    }
}
