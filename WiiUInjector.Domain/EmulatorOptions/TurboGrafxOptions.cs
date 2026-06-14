namespace WiiUInjector.Domain.EmulatorOptions
{
    /// <summary>
    /// Emulator options for TurboGrafx console.
    /// </summary>
    public sealed class TurboGrafxOptions : IEmulatorOptions
    {
        /// <summary>
        /// Whether this is a TurboGrafx CD game.
        /// </summary>
        public bool IsTurboCD { get; }

        /// <summary>
        /// The console these options apply to.
        /// </summary>
        public ConsoleType Console => ConsoleType.TurboGrafx;

        /// <summary>
        /// Constructs TurboGrafx emulator options with the specified settings.
        /// </summary>
        public TurboGrafxOptions(bool isTurboCD)
        {
            IsTurboCD = isTurboCD;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current TurboGrafxOptions.
        /// </summary>
        public override bool Equals(object obj) =>
            obj is TurboGrafxOptions other &&
            IsTurboCD == other.IsTurboCD;

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                return IsTurboCD.GetHashCode();
            }
        }

        /// <summary>
        /// Determines whether two TurboGrafxOptions instances are equal.
        /// </summary>
        public static bool operator ==(TurboGrafxOptions left, TurboGrafxOptions right) =>
            left?.Equals(right) ?? right is null;

        /// <summary>
        /// Determines whether two TurboGrafxOptions instances are not equal.
        /// </summary>
        public static bool operator !=(TurboGrafxOptions left, TurboGrafxOptions right) =>
            !(left == right);
    }
}
