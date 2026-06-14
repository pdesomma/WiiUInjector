namespace WiiUInjector.Domain.EmulatorOptions
{
    /// <summary>
    /// Factory for default emulator options per console.
    /// </summary>
    public static class EmulatorOptionsDefaults
    {
        /// <summary>
        /// Returns default emulator options for the specified console.
        /// A fresh instance is returned on each call.
        /// </summary>
        public static IEmulatorOptions For(ConsoleType console)
        {
            return console switch
            {
                ConsoleType.Nds => new DsOptions(DsLayout.Horizontal, StLayout.Both, 1, 50, PixelArtUpscaler.None),
                ConsoleType.N64 => new N64Options(false, false),
                ConsoleType.Gba => new GbaOptions(false, false),
                ConsoleType.Nes => new NesSnesOptions(ConsoleType.Nes, false),
                ConsoleType.Snes => new NesSnesOptions(ConsoleType.Snes, false),
                ConsoleType.TurboGrafx => new TurboGrafxOptions(false),
                ConsoleType.Msx => new NoOptions(ConsoleType.Msx),
                ConsoleType.Wii => new WiiOptions(false, false, false, false, false, false, false, false, false, ControllerLayout.GamePad, false),
                ConsoleType.Gcn => new GcnOptions(false, false, false),
                _ => throw new System.ArgumentOutOfRangeException(nameof(console), console, "No defaults defined for console.")
            };
        }
    }
}
