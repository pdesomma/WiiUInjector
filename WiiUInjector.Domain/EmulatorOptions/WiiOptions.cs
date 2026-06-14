namespace WiiUInjector.Domain.EmulatorOptions
{
    /// <summary>
    /// Emulator options for Wii console.
    /// </summary>
    public sealed class WiiOptions : IEmulatorOptions
    {
        /// <summary>
        /// Whether deflicker filter is removed.
        /// </summary>
        public bool RemoveDeflicker { get; }

        /// <summary>
        /// Whether dithering is removed.
        /// </summary>
        public bool RemoveDithering { get; }

        /// <summary>
        /// Whether half vertical filter is applied.
        /// </summary>
        public bool HalfVFilter { get; }

        /// <summary>
        /// Whether video mode patch is applied.
        /// </summary>
        public bool VideoModePatch { get; }

        /// <summary>
        /// Whether region is set to PAL.
        /// </summary>
        public bool ToPal { get; }

        /// <summary>
        /// Whether region-free USA patch is applied.
        /// </summary>
        public bool RegionFreeUs { get; }

        /// <summary>
        /// Whether region-free Japan patch is applied.
        /// </summary>
        public bool RegionFreeJp { get; }

        /// <summary>
        /// Whether region-free all patch is applied.
        /// </summary>
        public bool RegionFreeAll { get; }

        /// <summary>
        /// Whether motion controller data is passed through.
        /// </summary>
        public bool MotionPassthrough { get; }

        /// <summary>
        /// The controller layout configuration.
        /// </summary>
        public ControllerLayout ControllerLayout { get; }

        /// <summary>
        /// Whether L/R button patch is applied.
        /// </summary>
        public bool LRPatch { get; }

        /// <summary>
        /// The console these options apply to.
        /// </summary>
        public ConsoleType Console => ConsoleType.Wii;

        /// <summary>
        /// Constructs Wii emulator options with the specified settings.
        /// </summary>
        public WiiOptions(
            bool removeDeflicker,
            bool removeDithering,
            bool halfVFilter,
            bool videoModePatch,
            bool toPal,
            bool regionFreeUs,
            bool regionFreeJp,
            bool regionFreeAll,
            bool motionPassthrough,
            ControllerLayout controllerLayout,
            bool lrPatch)
        {
            RemoveDeflicker = removeDeflicker;
            RemoveDithering = removeDithering;
            HalfVFilter = halfVFilter;
            VideoModePatch = videoModePatch;
            ToPal = toPal;
            RegionFreeUs = regionFreeUs;
            RegionFreeJp = regionFreeJp;
            RegionFreeAll = regionFreeAll;
            MotionPassthrough = motionPassthrough;
            ControllerLayout = controllerLayout;
            LRPatch = lrPatch;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current WiiOptions.
        /// </summary>
        public override bool Equals(object obj) =>
            obj is WiiOptions other &&
            RemoveDeflicker == other.RemoveDeflicker &&
            RemoveDithering == other.RemoveDithering &&
            HalfVFilter == other.HalfVFilter &&
            VideoModePatch == other.VideoModePatch &&
            ToPal == other.ToPal &&
            RegionFreeUs == other.RegionFreeUs &&
            RegionFreeJp == other.RegionFreeJp &&
            RegionFreeAll == other.RegionFreeAll &&
            MotionPassthrough == other.MotionPassthrough &&
            ControllerLayout == other.ControllerLayout &&
            LRPatch == other.LRPatch;

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var h = RemoveDeflicker.GetHashCode();
                h = h * 397 ^ RemoveDithering.GetHashCode();
                h = h * 397 ^ HalfVFilter.GetHashCode();
                h = h * 397 ^ VideoModePatch.GetHashCode();
                h = h * 397 ^ ToPal.GetHashCode();
                h = h * 397 ^ RegionFreeUs.GetHashCode();
                h = h * 397 ^ RegionFreeJp.GetHashCode();
                h = h * 397 ^ RegionFreeAll.GetHashCode();
                h = h * 397 ^ MotionPassthrough.GetHashCode();
                h = h * 397 ^ ControllerLayout.GetHashCode();
                h = h * 397 ^ LRPatch.GetHashCode();
                return h;
            }
        }

        /// <summary>
        /// Determines whether two WiiOptions instances are equal.
        /// </summary>
        public static bool operator ==(WiiOptions left, WiiOptions right) =>
            left?.Equals(right) ?? right is null;

        /// <summary>
        /// Determines whether two WiiOptions instances are not equal.
        /// </summary>
        public static bool operator !=(WiiOptions left, WiiOptions right) =>
            !(left == right);
    }
}
