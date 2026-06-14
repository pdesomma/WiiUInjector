namespace WiiUInjector.Domain
{
    /// <summary>
    /// Immutable value object representing a DOL (Dolphin) executable section.
    /// </summary>
    public sealed class DolSection
    {
        /// <summary>
        /// Memory address where this section is loaded.
        /// </summary>
        public uint MemoryAddress { get; }

        /// <summary>
        /// Offset in the DOL file where this section begins.
        /// </summary>
        public uint FileOffset { get; }

        /// <summary>
        /// Size of the section in bytes.
        /// </summary>
        public uint Size { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DolSection"/> class.
        /// </summary>
        /// <param name="memoryAddress">Memory address where this section is loaded.</param>
        /// <param name="fileOffset">Offset in the DOL file where this section begins.</param>
        /// <param name="size">Size of the section in bytes.</param>
        public DolSection(uint memoryAddress, uint fileOffset, uint size)
        {
            MemoryAddress = memoryAddress;
            FileOffset = fileOffset;
            Size = size;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as DolSection;
            return other != null && MemoryAddress == other.MemoryAddress && FileOffset == other.FileOffset && Size == other.Size;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var h = (int)MemoryAddress;
                h = h * 397 ^ (int)FileOffset;
                h = h * 397 ^ (int)Size;
                return h;
            }
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="DolSection"/> are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>True if left and right are equal; otherwise, false.</returns>
        public static bool operator ==(DolSection left, DolSection right)
        {
            return ReferenceEquals(left, right) || (left != null && left.Equals(right));
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="DolSection"/> are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>True if left and right are not equal; otherwise, false.</returns>
        public static bool operator !=(DolSection left, DolSection right)
        {
            return !(left == right);
        }
    }
}
