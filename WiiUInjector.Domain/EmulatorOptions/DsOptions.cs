namespace WiiUInjector.Domain.EmulatorOptions
{
    /// <summary>
    /// Emulator options for Nintendo DS console.
    /// </summary>
    public sealed class DsOptions : IEmulatorOptions
    {
        /// <summary>
        /// The display layout configuration for DS screens.
        /// </summary>
        public DsLayout DsLayout { get; }

        /// <summary>
        /// The display layout configuration for stylus/touch screen.
        /// </summary>
        public StLayout StLayout { get; }

        /// <summary>
        /// The renderer scale factor (must be greater than 0).
        /// </summary>
        public int RendererScale { get; }

        /// <summary>
        /// The screen brightness level (0-100).
        /// </summary>
        public int Brightness { get; }

        /// <summary>
        /// The pixel art upscaling algorithm.
        /// </summary>
        public PixelArtUpscaler PixelArtUpscaler { get; }

        /// <summary>
        /// The console these options apply to.
        /// </summary>
        public ConsoleType Console => ConsoleType.Nds;

        /// <summary>
        /// Constructs Nintendo DS emulator options with the specified settings.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when RendererScale is less than or equal to 0, or Brightness is less than 0 or greater than 100.</exception>
        public DsOptions(
            DsLayout dsLayout,
            StLayout stLayout,
            int rendererScale,
            int brightness,
            PixelArtUpscaler pixelArtUpscaler)
        {
            if (rendererScale <= 0)
                throw new System.ArgumentOutOfRangeException(nameof(rendererScale), rendererScale, "RendererScale must be greater than 0.");
            if (brightness < 0 || brightness > 100)
                throw new System.ArgumentOutOfRangeException(nameof(brightness), brightness, "Brightness must be between 0 and 100.");
            DsLayout = dsLayout;
            StLayout = stLayout;
            RendererScale = rendererScale;
            Brightness = brightness;
            PixelArtUpscaler = pixelArtUpscaler;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current DsOptions.
        /// </summary>
        public override bool Equals(object obj) =>
            obj is DsOptions other &&
            DsLayout == other.DsLayout &&
            StLayout == other.StLayout &&
            RendererScale == other.RendererScale &&
            Brightness == other.Brightness &&
            PixelArtUpscaler == other.PixelArtUpscaler;

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var h = DsLayout.GetHashCode();
                h = h * 397 ^ StLayout.GetHashCode();
                h = h * 397 ^ RendererScale.GetHashCode();
                h = h * 397 ^ Brightness.GetHashCode();
                h = h * 397 ^ PixelArtUpscaler.GetHashCode();
                return h;
            }
        }

        /// <summary>
        /// Determines whether two DsOptions instances are equal.
        /// </summary>
        public static bool operator ==(DsOptions left, DsOptions right) =>
            left?.Equals(right) ?? right is null;

        /// <summary>
        /// Determines whether two DsOptions instances are not equal.
        /// </summary>
        public static bool operator !=(DsOptions left, DsOptions right) =>
            !(left == right);
    }
}
