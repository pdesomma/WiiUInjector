namespace WiiUInjector.Domain.EmulatorOptions
{
    /// <summary>
    /// Emulator options for Nintendo Entertainment System and Super Nintendo Entertainment System consoles.
    /// </summary>
    public sealed class NesSnesOptions : IEmulatorOptions
    {
        /// <summary>
        /// The console these options apply to.
        /// </summary>
        public ConsoleType Console { get; }

        /// <summary>
        /// Whether pixel-perfect rendering is enabled.
        /// </summary>
        public bool PixelPerfect { get; }

        /// <summary>
        /// Constructs NES/SNES emulator options with the specified settings.
        /// </summary>
        /// <exception cref="System.ArgumentException">Thrown when console is not NES or SNES.</exception>
        public NesSnesOptions(ConsoleType console, bool pixelPerfect)
        {
            if (console != ConsoleType.Nes && console != ConsoleType.Snes)
                throw new System.ArgumentException("NesSnesOptions only supports Nes or Snes.", nameof(console));
            Console = console;
            PixelPerfect = pixelPerfect;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current NesSnesOptions.
        /// </summary>
        public override bool Equals(object obj) =>
            obj is NesSnesOptions other &&
            Console == other.Console &&
            PixelPerfect == other.PixelPerfect;

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var h = Console.GetHashCode();
                h = h * 397 ^ PixelPerfect.GetHashCode();
                return h;
            }
        }

        /// <summary>
        /// Determines whether two NesSnesOptions instances are equal.
        /// </summary>
        public static bool operator ==(NesSnesOptions left, NesSnesOptions right) =>
            left?.Equals(right) ?? right is null;

        /// <summary>
        /// Determines whether two NesSnesOptions instances are not equal.
        /// </summary>
        public static bool operator !=(NesSnesOptions left, NesSnesOptions right) =>
            !(left == right);
    }
}
