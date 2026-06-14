using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WiiUInjector.Application.Abstractions.Environment
{
    /// <summary>
    /// A sealed value object capturing the current system environment conditions
    /// that affect application startup and operation.
    /// </summary>
    public sealed class EnvironmentReport
    {
        /// <summary>
        /// Gets a value indicating whether the application is running from a OneDrive path.
        /// </summary>
        public bool IsRunningFromOneDrive { get; }

        /// <summary>
        /// Gets a value indicating whether another instance of the application is already running.
        /// </summary>
        public bool IsDuplicateInstance { get; }

        /// <summary>
        /// Gets a value indicating whether the current screen meets the minimum resolution requirement (1152x864).
        /// </summary>
        public bool ScreenMeetsMinimumResolution { get; }

        /// <summary>
        /// Gets the width of the current screen in pixels.
        /// </summary>
        public int ScreenWidth { get; }

        /// <summary>
        /// Gets the height of the current screen in pixels.
        /// </summary>
        public int ScreenHeight { get; }

        /// <summary>
        /// Gets a value indicating whether the application is running under a Wine, Proton, CrossOver, or similar compatibility layer.
        /// </summary>
        public bool IsRunningUnderWineOrSimilar { get; }

        /// <summary>
        /// Gets a read-only collection of human-readable notes about the environment conditions.
        /// </summary>
        public IReadOnlyList<string> Notes { get; }

        /// <summary>
        /// Initializes a new EnvironmentReport with the specified environment flags, screen dimensions, and notes.
        /// </summary>
        /// <param name="isRunningFromOneDrive">Indicates whether the app is running from OneDrive.</param>
        /// <param name="isDuplicateInstance">Indicates whether a duplicate instance is running.</param>
        /// <param name="screenMeetsMinimumResolution">Indicates whether the screen meets the minimum resolution.</param>
        /// <param name="screenWidth">The current screen width in pixels.</param>
        /// <param name="screenHeight">The current screen height in pixels.</param>
        /// <param name="isRunningUnderWineOrSimilar">Indicates whether running under a compatibility layer.</param>
        /// <param name="notes">A list of human-readable environment notes; must not be null (defensive copy will be made).</param>
        /// <exception cref="ArgumentNullException">Thrown if notes is null.</exception>
        public EnvironmentReport(
            bool isRunningFromOneDrive,
            bool isDuplicateInstance,
            bool screenMeetsMinimumResolution,
            int screenWidth,
            int screenHeight,
            bool isRunningUnderWineOrSimilar,
            IList<string> notes)
        {
            if (notes == null)
                throw new ArgumentNullException(nameof(notes));

            IsRunningFromOneDrive = isRunningFromOneDrive;
            IsDuplicateInstance = isDuplicateInstance;
            ScreenMeetsMinimumResolution = screenMeetsMinimumResolution;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            IsRunningUnderWineOrSimilar = isRunningUnderWineOrSimilar;
            Notes = new ReadOnlyCollection<string>(notes);
        }
    }
}
