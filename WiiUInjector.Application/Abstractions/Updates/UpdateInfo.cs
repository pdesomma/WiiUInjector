using System;

namespace WiiUInjector.Application.Abstractions.Updates
{
    /// <summary>
    /// A sealed value object containing information about an available application update.
    /// </summary>
    public sealed class UpdateInfo
    {
        /// <summary>
        /// Gets the current version of the application.
        /// </summary>
        public string CurrentVersion { get; }

        /// <summary>
        /// Gets the latest available version.
        /// </summary>
        public string LatestVersion { get; }

        /// <summary>
        /// Gets the URL where the release can be accessed or downloaded.
        /// </summary>
        public string ReleaseUrl { get; }

        /// <summary>
        /// Gets the release notes for the latest version, or null if unavailable.
        /// </summary>
        public string ReleaseNotes { get; }

        /// <summary>
        /// Gets a value indicating whether a newer version is available.
        /// </summary>
        public bool IsNewerAvailable { get; }

        /// <summary>
        /// Initializes a new UpdateInfo with the specified version information and update availability flag.
        /// </summary>
        /// <param name="currentVersion">The current application version; must not be null or whitespace.</param>
        /// <param name="latestVersion">The latest available version; must not be null or whitespace.</param>
        /// <param name="releaseUrl">The URL for the release; must not be null or whitespace.</param>
        /// <param name="isNewerAvailable">A flag indicating whether a newer version is available.</param>
        /// <param name="releaseNotes">Optional release notes; may be null or empty.</param>
        /// <exception cref="ArgumentNullException">Thrown if currentVersion, latestVersion, or releaseUrl is null.</exception>
        /// <exception cref="ArgumentException">Thrown if currentVersion, latestVersion, or releaseUrl is empty or whitespace.</exception>
        public UpdateInfo(
            string currentVersion,
            string latestVersion,
            string releaseUrl,
            bool isNewerAvailable,
            string releaseNotes = null)
        {
            if (currentVersion == null)
                throw new ArgumentNullException(nameof(currentVersion));
            if (string.IsNullOrWhiteSpace(currentVersion))
                throw new ArgumentException("Current version cannot be empty or whitespace.", nameof(currentVersion));

            if (latestVersion == null)
                throw new ArgumentNullException(nameof(latestVersion));
            if (string.IsNullOrWhiteSpace(latestVersion))
                throw new ArgumentException("Latest version cannot be empty or whitespace.", nameof(latestVersion));

            if (releaseUrl == null)
                throw new ArgumentNullException(nameof(releaseUrl));
            if (string.IsNullOrWhiteSpace(releaseUrl))
                throw new ArgumentException("Release URL cannot be empty or whitespace.", nameof(releaseUrl));

            CurrentVersion = currentVersion;
            LatestVersion = latestVersion;
            ReleaseUrl = releaseUrl;
            IsNewerAvailable = isNewerAvailable;
            ReleaseNotes = releaseNotes;
        }
    }
}
