using System;

namespace WiiUInjector.Application.Abstractions.Settings
{
    /// <summary>
    /// A POCO representing application settings persisted across sessions.
    /// Mirrors the legacy JsonAppSettings structure.
    /// </summary>
    public class ApplicationSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the base and output paths have been configured.
        /// </summary>
        public bool PathsSet { get; set; }

        /// <summary>
        /// Gets or sets the base ROM directory path.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// Gets or sets the output directory path for injection results.
        /// </summary>
        public string OutPath { get; set; }

        /// <summary>
        /// Gets or sets the common key used for encrypted base ROM packages.
        /// </summary>
        public string Ckey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the base path should only be set once during setup.
        /// </summary>
        public bool SetBaseOnce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the output path should only be set once during setup.
        /// </summary>
        public bool SetOutOnce { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an upgrade is required for the application.
        /// </summary>
        public bool UpgradeRequired { get; set; }

        /// <summary>
        /// Gets or sets the system key used for certain console operations.
        /// </summary>
        public string SysKey { get; set; }

        /// <summary>
        /// Gets or sets an alternate system key.
        /// </summary>
        public string SysKey1 { get; set; }

        /// <summary>
        /// Gets or sets a flag controlling DS injection behavior (legacy name; indicates whether to disable certain DS features).
        /// </summary>
        public bool dont { get; set; }

        /// <summary>
        /// Gets or sets a flag controlling NDS Wii injection behavior.
        /// </summary>
        public bool ndsw { get; set; }

        /// <summary>
        /// Gets or sets a flag controlling SNES Wii injection behavior.
        /// </summary>
        public bool snesw { get; set; }

        /// <summary>
        /// Gets or sets a flag controlling GameCube Wii injection behavior.
        /// </summary>
        public bool gczw { get; set; }

        /// <summary>
        /// Gets or sets the Ancast (boot sound) file path or reference.
        /// </summary>
        public string Ancast { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the first launch of the application.
        /// </summary>
        public bool IsFirstLaunch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user should be shown a prompt about the Zesty TS fork.
        /// </summary>
        public bool ShowZestyFork { get; set; }

        /// <summary>
        /// Initializes a new ApplicationSettings instance with default values.
        /// </summary>
        public ApplicationSettings()
        {
            PathsSet = false;
            BasePath = "";
            OutPath = "";
            Ckey = "";
            SetBaseOnce = false;
            SetOutOnce = false;
            UpgradeRequired = true;
            SysKey = "";
            SysKey1 = "";
            dont = false;
            ndsw = false;
            snesw = false;
            gczw = false;
            Ancast = "";
            IsFirstLaunch = true;
            ShowZestyFork = true;
        }

        /// <summary>
        /// Initializes a new ApplicationSettings instance with all specified values.
        /// </summary>
        /// <param name="pathsSet">Indicates whether paths have been set.</param>
        /// <param name="basePath">The base ROM directory path.</param>
        /// <param name="outPath">The output directory path.</param>
        /// <param name="ckey">The common key.</param>
        /// <param name="setBaseOnce">Indicates whether the base path should only be set once.</param>
        /// <param name="setOutOnce">Indicates whether the output path should only be set once.</param>
        /// <param name="upgradeRequired">Indicates whether an upgrade is required.</param>
        /// <param name="sysKey">The system key.</param>
        /// <param name="sysKey1">The alternate system key.</param>
        /// <param name="dontValue">DS injection behavior flag.</param>
        /// <param name="ndswValue">NDS Wii injection behavior flag.</param>
        /// <param name="sneswValue">SNES Wii injection behavior flag.</param>
        /// <param name="gczwValue">GameCube Wii injection behavior flag.</param>
        /// <param name="ancast">The Ancast file path or reference.</param>
        /// <param name="isFirstLaunch">Indicates whether this is the first launch.</param>
        /// <param name="showZestyFork">Indicates whether to show the Zesty TS fork prompt.</param>
        public ApplicationSettings(
            bool pathsSet,
            string basePath,
            string outPath,
            string ckey,
            bool setBaseOnce,
            bool setOutOnce,
            bool upgradeRequired,
            string sysKey,
            string sysKey1,
            bool dontValue,
            bool ndswValue,
            bool sneswValue,
            bool gczwValue,
            string ancast,
            bool isFirstLaunch,
            bool showZestyFork)
        {
            PathsSet = pathsSet;
            BasePath = basePath;
            OutPath = outPath;
            Ckey = ckey;
            SetBaseOnce = setBaseOnce;
            SetOutOnce = setOutOnce;
            UpgradeRequired = upgradeRequired;
            SysKey = sysKey;
            SysKey1 = sysKey1;
            this.dont = dontValue;
            this.ndsw = ndswValue;
            this.snesw = sneswValue;
            this.gczw = gczwValue;
            Ancast = ancast;
            IsFirstLaunch = isFirstLaunch;
            ShowZestyFork = showZestyFork;
        }
    }
}
