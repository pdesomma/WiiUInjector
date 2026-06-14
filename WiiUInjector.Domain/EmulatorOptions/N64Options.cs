namespace WiiUInjector.Domain.EmulatorOptions
{
    /// <summary>
    /// Emulator options for Nintendo 64 console.
    /// </summary>
    public sealed class N64Options : IEmulatorOptions
    {
        /// <summary>
        /// Whether dark filter is enabled.
        /// </summary>
        public bool DarkFilter { get; }

        /// <summary>
        /// Whether widescreen mode is enabled.
        /// </summary>
        public bool WideScreen { get; }

        /// <summary>
        /// The console these options apply to.
        /// </summary>
        public ConsoleType Console => ConsoleType.N64;

        /// <summary>
        /// Constructs N64 emulator options with the specified settings.
        /// </summary>
        public N64Options(bool darkFilter, bool wideScreen)
        {
            DarkFilter = darkFilter;
            WideScreen = wideScreen;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current N64Options.
        /// </summary>
        public override bool Equals(object obj) =>
            obj is N64Options other &&
            DarkFilter == other.DarkFilter &&
            WideScreen == other.WideScreen;

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var h = DarkFilter.GetHashCode();
                h = h * 397 ^ WideScreen.GetHashCode();
                return h;
            }
        }

        /// <summary>
        /// Determines whether two N64Options instances are equal.
        /// </summary>
        public static bool operator ==(N64Options left, N64Options right) =>
            left?.Equals(right) ?? right is null;

        /// <summary>
        /// Determines whether two N64Options instances are not equal.
        /// </summary>
        public static bool operator !=(N64Options left, N64Options right) =>
            !(left == right);
    }
}
