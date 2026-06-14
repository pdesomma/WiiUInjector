using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WiiUInjector.Domain
{
    /// <summary>
    /// Immutable value object holding raw image bytes plus metadata.
    /// </summary>
    public sealed class GameImage
    {
        /// <summary>
        /// The logical slot this image occupies (Icon, Drc, Tv, or Logo).
        /// </summary>
        public ImageSlot Slot { get; }

        /// <summary>
        /// Image width in pixels.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Image height in pixels.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Bits per pixel (e.g., 24 for RGB, 32 for RGBA).
        /// </summary>
        public int BitDepth { get; }

        /// <summary>
        /// Raw image bytes; read-only wrapper over an internal defensive copy.
        /// </summary>
        public IReadOnlyList<byte> Bytes { get; }

        /// <summary>
        /// Constructs an immutable GameImage with defensive copying of the byte array.
        /// </summary>
        /// <param name="slot">The logical slot this image occupies.</param>
        /// <param name="width">Image width; must be positive.</param>
        /// <param name="height">Image height; must be positive.</param>
        /// <param name="bitDepth">Bits per pixel; must be positive.</param>
        /// <param name="bytes">Raw image bytes; cannot be null or empty.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="bytes"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="bytes"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if width, height, or bitDepth is not positive.</exception>
        public GameImage(ImageSlot slot, int width, int height, int bitDepth, byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length == 0)
                throw new ArgumentException("Image bytes cannot be empty.", nameof(bytes));

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");

            if (bitDepth <= 0)
                throw new ArgumentOutOfRangeException(nameof(bitDepth), "BitDepth must be positive.");

            var copy = new byte[bytes.Length];
            Array.Copy(bytes, copy, bytes.Length);

            Slot = slot;
            Width = width;
            Height = height;
            BitDepth = bitDepth;
            Bytes = new ReadOnlyCollection<byte>(copy);
        }

        /// <summary>
        /// Structural equality comparing slot, dimensions, bit depth, and bytes.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is GameImage other))
                return false;

            if (Slot != other.Slot || Width != other.Width || Height != other.Height || BitDepth != other.BitDepth)
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
        /// Hash code combining slot, dimensions, bit depth, and byte array length.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Slot.GetHashCode();
                hash = hash * 31 + Width.GetHashCode();
                hash = hash * 31 + Height.GetHashCode();
                hash = hash * 31 + BitDepth.GetHashCode();
                hash = hash * 31 + Bytes.Count.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// Structural equality operator.
        /// </summary>
        public static bool operator ==(GameImage left, GameImage right)
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
        public static bool operator !=(GameImage left, GameImage right)
        {
            return !(left == right);
        }
    }
}
