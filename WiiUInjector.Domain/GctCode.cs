namespace WiiUInjector.Domain
{
    /// <summary>
    /// Immutable value object representing a GCT (Game Configuration Table) code entry.
    /// </summary>
    public sealed class GctCode
    {
        /// <summary>
        /// Memory address for the code.
        /// </summary>
        public uint Address { get; }

        /// <summary>
        /// Value to write at the address.
        /// </summary>
        public uint Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GctCode"/> class.
        /// </summary>
        /// <param name="address">Memory address for the code.</param>
        /// <param name="value">Value to write at the address.</param>
        public GctCode(uint address, uint value)
        {
            Address = address;
            Value = value;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as GctCode;
            return other != null && Address == other.Address && Value == other.Value;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (int)(Address * 397) ^ (int)Value;
            }
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GctCode"/> are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>True if left and right are equal; otherwise, false.</returns>
        public static bool operator ==(GctCode left, GctCode right)
        {
            return ReferenceEquals(left, right) || (left != null && left.Equals(right));
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GctCode"/> are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns>True if left and right are not equal; otherwise, false.</returns>
        public static bool operator !=(GctCode left, GctCode right)
        {
            return !(left == right);
        }
    }
}
