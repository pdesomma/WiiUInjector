namespace WiiUInjector.Domain.EmulatorOptions
{
    /// <summary>
    /// Emulator options for Game Boy Advance console.
    /// </summary>
    public sealed class GbaOptions : IEmulatorOptions
    {
        /// <summary>
        /// Whether Pokemon compatibility patch is applied.
        /// </summary>
        public bool PokemonPatch { get; }

        /// <summary>
        /// Whether dark filter is removed.
        /// </summary>
        public bool DarkFilterRemoval { get; }

        /// <summary>
        /// The console these options apply to.
        /// </summary>
        public ConsoleType Console => ConsoleType.Gba;

        /// <summary>
        /// Constructs Game Boy Advance emulator options with the specified settings.
        /// </summary>
        public GbaOptions(bool pokemonPatch, bool darkFilterRemoval)
        {
            PokemonPatch = pokemonPatch;
            DarkFilterRemoval = darkFilterRemoval;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current GbaOptions.
        /// </summary>
        public override bool Equals(object obj) =>
            obj is GbaOptions other &&
            PokemonPatch == other.PokemonPatch &&
            DarkFilterRemoval == other.DarkFilterRemoval;

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var h = PokemonPatch.GetHashCode();
                h = h * 397 ^ DarkFilterRemoval.GetHashCode();
                return h;
            }
        }

        /// <summary>
        /// Determines whether two GbaOptions instances are equal.
        /// </summary>
        public static bool operator ==(GbaOptions left, GbaOptions right) =>
            left?.Equals(right) ?? right is null;

        /// <summary>
        /// Determines whether two GbaOptions instances are not equal.
        /// </summary>
        public static bool operator !=(GbaOptions left, GbaOptions right) =>
            !(left == right);
    }
}
