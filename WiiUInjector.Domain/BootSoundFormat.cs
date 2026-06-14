namespace WiiUInjector.Domain
{
    /// <summary>
    /// Audio format for boot sounds.
    /// </summary>
    public enum BootSoundFormat
    {
        /// <summary>
        /// MPEG-3 audio.
        /// </summary>
        Mp3,
        /// <summary>
        /// Waveform audio.
        /// </summary>
        Wav,
        /// <summary>
        /// Pre-converted native Wii U boot sound binary.
        /// </summary>
        BtsndNative
    }
}
