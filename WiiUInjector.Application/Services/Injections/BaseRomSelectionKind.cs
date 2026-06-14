namespace WiiUInjector.Application.Services.Injections
{
    /// <summary>
    /// Discriminator for base ROM selection kind (curated catalog entry or custom file).
    /// </summary>
    public enum BaseRomSelectionKind
    {
        /// <summary>
        /// A curated base ROM entry from the catalog.
        /// </summary>
        Curated,

        /// <summary>
        /// A custom user-supplied base ROM file.
        /// </summary>
        Custom
    }
}
