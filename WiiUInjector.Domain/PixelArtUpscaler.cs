namespace WiiUInjector.Domain
{
    /// <summary>
    /// Pixel art upscaling algorithms.
    /// </summary>
    public enum PixelArtUpscaler
    {
        /// <summary>
        /// No upscaling.
        /// </summary>
        None,
        /// <summary>
        /// HQ2x algorithm.
        /// </summary>
        Hq2x,
        /// <summary>
        /// HQ4x algorithm.
        /// </summary>
        Hq4x,
        /// <summary>
        /// Scale2x algorithm.
        /// </summary>
        Scale2x,
        /// <summary>
        /// Scale4x algorithm.
        /// </summary>
        Scale4x
    }
}
