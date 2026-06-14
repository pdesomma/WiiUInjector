namespace WiiUInjector.Domain.EmulatorOptions
{
    /// <summary>
    /// A marker for consoles with no emulator-specific options.
    /// </summary>
    public sealed class NoOptions : IEmulatorOptions
    {
        /// <summary>
        /// The console these (absent) options apply to.
        /// </summary>
        public ConsoleType Console { get; }

        /// <summary>
        /// Constructs a no-options marker for the given console.
        /// </summary>
        public NoOptions(ConsoleType console)
        {
            Console = console;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current NoOptions.
        /// </summary>
        public override bool Equals(object obj) =>
            obj is NoOptions other && other.Console == Console;

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode() => (int)Console;

        /// <summary>
        /// Determines whether two NoOptions instances are equal.
        /// </summary>
        public static bool operator ==(NoOptions left, NoOptions right) =>
            left?.Equals(right) ?? right is null;

        /// <summary>
        /// Determines whether two NoOptions instances are not equal.
        /// </summary>
        public static bool operator !=(NoOptions left, NoOptions right) =>
            !(left == right);
    }
}
