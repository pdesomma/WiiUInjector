namespace WiiUInjector.Application.Abstractions.Operations
{
    /// <summary>
    /// Enumeration of disc image formats supported for ROM transcoding.
    /// </summary>
    public enum DiscImageFormat
    {
        /// <summary>
        /// ISO 9660 format (standard disc image).
        /// </summary>
        Iso,

        /// <summary>
        /// NKIT format (optimized Nintendo disc image).
        /// </summary>
        Nkit,

        /// <summary>
        /// WBFS format (Wii Backup File System).
        /// </summary>
        Wbfs,

        /// <summary>
        /// GCZ format (compressed GameCube disc image).
        /// </summary>
        Gcz
    }
}
