namespace WiiUInjector.Domain.EmulatorOptions
{
    /// <summary>
    /// Emulator options for GameCube console.
    /// </summary>
    public sealed class GcnOptions : IEmulatorOptions
    {
        /// <summary>
        /// Whether disc trimming is disabled.
        /// </summary>
        public bool DontTrim { get; }

        /// <summary>
        /// Whether this is a multi-disc game (disc 2).
        /// </summary>
        public bool Disc2 { get; }

        /// <summary>
        /// Whether 4:3 aspect ratio is forced.
        /// </summary>
        public bool Force4x3 { get; }

        /// <summary>
        /// The console these options apply to.
        /// </summary>
        public ConsoleType Console => ConsoleType.Gcn;

        /// <summary>
        /// Constructs GameCube emulator options with the specified settings.
        /// </summary>
        public GcnOptions(bool dontTrim, bool disc2, bool force4x3)
        {
            DontTrim = dontTrim;
            Disc2 = disc2;
            Force4x3 = force4x3;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current GcnOptions.
        /// </summary>
        public override bool Equals(object obj) =>
            obj is GcnOptions other &&
            DontTrim == other.DontTrim &&
            Disc2 == other.Disc2 &&
            Force4x3 == other.Force4x3;

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var h = DontTrim.GetHashCode();
                h = h * 397 ^ Disc2.GetHashCode();
                h = h * 397 ^ Force4x3.GetHashCode();
                return h;
            }
        }

        /// <summary>
        /// Determines whether two GcnOptions instances are equal.
        /// </summary>
        public static bool operator ==(GcnOptions left, GcnOptions right) => left?.Equals(right) ?? right is null;

        /// <summary>
        /// Determines whether two GcnOptions instances are not equal.
        /// </summary>
        public static bool operator !=(GcnOptions left, GcnOptions right) => !(left == right);
    }
}
