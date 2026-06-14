using System;
using System.Collections.Generic;
using WiiUInjector.Domain.EmulatorOptions;

namespace WiiUInjector.Domain.Services
{
    /// <summary>
    /// Validates business rules on Injection that constructor invariants don't enforce: image dimensions, ROM size caps, and emulator option combinations.
    /// </summary>
    public sealed class InjectionValidator : IInjectionValidator
    {
        /// <summary>
        /// Required width in pixels for Icon slot images.
        /// </summary>
        private const int IconWidth = 128;

        /// <summary>
        /// Required height in pixels for Icon slot images.
        /// </summary>
        private const int IconHeight = 128;

        /// <summary>
        /// Required bit depth for Icon slot images.
        /// </summary>
        private const int IconBitDepth = 32;

        /// <summary>
        /// Required width in pixels for Drc slot images.
        /// </summary>
        private const int DrcWidth = 854;

        /// <summary>
        /// Required height in pixels for Drc slot images.
        /// </summary>
        private const int DrcHeight = 480;

        /// <summary>
        /// Required bit depth for Drc slot images.
        /// </summary>
        private const int DrcBitDepth = 24;

        /// <summary>
        /// Required width in pixels for Tv slot images.
        /// </summary>
        private const int TvWidth = 1280;

        /// <summary>
        /// Required height in pixels for Tv slot images.
        /// </summary>
        private const int TvHeight = 720;

        /// <summary>
        /// Required bit depth for Tv slot images.
        /// </summary>
        private const int TvBitDepth = 24;

        /// <summary>
        /// Required width in pixels for Logo slot images.
        /// </summary>
        private const int LogoWidth = 170;

        /// <summary>
        /// Required height in pixels for Logo slot images.
        /// </summary>
        private const int LogoHeight = 42;

        /// <summary>
        /// Required bit depth for Logo slot images.
        /// </summary>
        private const int LogoBitDepth = 32;

        /// <summary>
        /// Maximum ROM sizes per console type, enforcing reasonable limits from legacy hardware constraints.
        /// </summary>
        private static readonly Dictionary<ConsoleType, long> RomSizeCaps =
            new Dictionary<ConsoleType, long>
            {
                { ConsoleType.Nes,         1 * 1024 * 1024 },
                { ConsoleType.Snes,        8 * 1024 * 1024 },
                { ConsoleType.Gba,        32 * 1024 * 1024 },
                { ConsoleType.N64,        64 * 1024 * 1024 },
                { ConsoleType.Nds,       512L * 1024 * 1024 },
                { ConsoleType.TurboGrafx,  4 * 1024 * 1024 },
                { ConsoleType.Msx,         4 * 1024 * 1024 },
                { ConsoleType.Wii,      4700L * 1024 * 1024 },
                { ConsoleType.Gcn,      1500L * 1024 * 1024 },
            };

        /// <summary>
        /// Validates the injection against all business rules: image dimensions, ROM size, and emulator option combinations.
        /// Collects all violations and returns them without throwing.
        /// </summary>
        /// <param name="injection">The injection to validate; must not be null.</param>
        /// <returns>A ValidationResult containing all rule violations.</returns>
        /// <exception cref="ArgumentNullException">Thrown if injection is null.</exception>
        public ValidationResult Validate(Injection injection)
        {
            if (injection == null)
                throw new ArgumentNullException(nameof(injection));

            var errors = new List<ValidationError>();

            ValidateImages(injection, errors);
            ValidateRomSize(injection, errors);
            ValidateWiiOptionCombos(injection, errors);

            return new ValidationResult(errors);
        }

        /// <summary>
        /// Validates that all four image slots match their required dimensions and bit depth.
        /// </summary>
        private static void ValidateImages(Injection injection, List<ValidationError> errors)
        {
            ValidateImageSlot(injection, ImageSlot.Icon, IconWidth, IconHeight, IconBitDepth, "ICON_DIMENSIONS", errors);
            ValidateImageSlot(injection, ImageSlot.Drc, DrcWidth, DrcHeight, DrcBitDepth, "DRC_DIMENSIONS", errors);
            ValidateImageSlot(injection, ImageSlot.Tv, TvWidth, TvHeight, TvBitDepth, "TV_DIMENSIONS", errors);
            ValidateImageSlot(injection, ImageSlot.Logo, LogoWidth, LogoHeight, LogoBitDepth, "LOGO_DIMENSIONS", errors);
        }

        /// <summary>
        /// Validates a single image slot's dimensions and bit depth.
        /// </summary>
        private static void ValidateImageSlot(Injection injection, ImageSlot slot, int expectedWidth, int expectedHeight, int expectedBitDepth, string code, List<ValidationError> errors)
        {
            if (!injection.Images.ContainsKey(slot))
                return;

            var image = injection.Images[slot];

            if (image.Width != expectedWidth || image.Height != expectedHeight || image.BitDepth != expectedBitDepth)
            {
                var error = new ValidationError(
                    code,
                    $"{slot} image must be {expectedWidth}x{expectedHeight}x{expectedBitDepth}-bit; got {image.Width}x{image.Height}x{image.BitDepth}.",
                    $"Images[{slot}]");

                errors.Add(error);
            }
        }

        /// <summary>
        /// Validates that ROM size does not exceed the cap for its console type.
        /// </summary>
        private static void ValidateRomSize(Injection injection, List<ValidationError> errors)
        {
            if (!RomSizeCaps.TryGetValue(injection.Console, out long cap))
                return;

            if (injection.Rom.Length > cap)
            {
                var error = new ValidationError(
                    "ROM_SIZE",
                    $"ROM exceeds {cap:N0}-byte cap for {injection.Console}: actual {injection.Rom.Length:N0} bytes.",
                    "Rom.Length");

                errors.Add(error);
            }
        }

        /// <summary>
        /// Validates Wii-specific option combinations.
        /// </summary>
        private static void ValidateWiiOptionCombos(Injection injection, List<ValidationError> errors)
        {
            if (!(injection.Options is WiiOptions wii))
                return;

            if (wii.ToPal && !wii.VideoModePatch)
            {
                var error = new ValidationError(
                    "WII_TO_PAL_REQUIRES_VIDEO_PATCH",
                    "Wii ToPal option requires VideoModePatch to also be enabled.",
                    "Options.ToPal");

                errors.Add(error);
            }

            if (wii.RegionFreeAll && (wii.RegionFreeUs || wii.RegionFreeJp))
            {
                var error = new ValidationError(
                    "WII_REGIONFREE_CONFLICT",
                    "RegionFreeAll cannot combine with RegionFreeUs or RegionFreeJp.",
                    "Options");

                errors.Add(error);
            }
        }
    }
}
