namespace WiiUInjector.Domain.EmulatorOptions
{
    /// <summary>
    /// Marker interface for per-console emulator options. The <see cref="Console"/> property
    /// lets the Injection aggregate verify the options match the selected console at construction time.
    /// </summary>
    public interface IEmulatorOptions
    {
        /// <summary>
        /// The console these options apply to.
        /// </summary>
        ConsoleType Console { get; }
    }
}
