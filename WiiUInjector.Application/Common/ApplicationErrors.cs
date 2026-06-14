namespace WiiUInjector.Application.Common
{
    /// <summary>
    /// Static class defining well-known error codes for application-layer failures.
    /// </summary>
    public static class ApplicationErrors
    {
        /// <summary>
        /// Error code for when a ROM file is not found.
        /// </summary>
        public const string RomNotFound = "ROM_NOT_FOUND";

        /// <summary>
        /// Error code for when a ROM file cannot be read.
        /// </summary>
        public const string RomReadFailed = "ROM_READ_FAILED";

        /// <summary>
        /// Error code for when an image file is not found.
        /// </summary>
        public const string ImageNotFound = "IMAGE_NOT_FOUND";

        /// <summary>
        /// Error code for when an image file cannot be decoded.
        /// </summary>
        public const string ImageDecodeFailed = "IMAGE_DECODE_FAILED";

        /// <summary>
        /// Error code for when a boot sound file is not found.
        /// </summary>
        public const string BootSoundNotFound = "BOOT_SOUND_NOT_FOUND";

        /// <summary>
        /// Error code for when a boot sound file cannot be decoded.
        /// </summary>
        public const string BootSoundDecodeFailed = "BOOT_SOUND_DECODE_FAILED";

        /// <summary>
        /// Error code for when a base ROM is not found in the catalog.
        /// </summary>
        public const string BaseRomNotInCatalog = "BASE_ROM_NOT_IN_CATALOG";

        /// <summary>
        /// Error code for when a custom base ROM is invalid.
        /// </summary>
        public const string CustomBaseRomInvalid = "CUSTOM_BASE_ROM_INVALID";

        /// <summary>
        /// Error code for when emulator options do not match the target console.
        /// </summary>
        public const string EmulatorOptionsConsoleMismatch = "EMULATOR_OPTIONS_CONSOLE_MISMATCH";

        /// <summary>
        /// Error code for when a domain-layer invariant is violated.
        /// </summary>
        public const string DomainInvariantViolation = "DOMAIN_INVARIANT_VIOLATION";

        /// <summary>
        /// Error code for when validation fails.
        /// </summary>
        public const string ValidationFailed = "VALIDATION_FAILED";

        /// <summary>
        /// Error code for when injection workspace creation fails.
        /// </summary>
        public const string WorkspaceCreateFailed = "WORKSPACE_CREATE_FAILED";

        /// <summary>
        /// Error code for when base ROM extraction fails.
        /// </summary>
        public const string BaseRomExtractFailed = "BASE_ROM_EXTRACT_FAILED";

        /// <summary>
        /// Error code for when ROM injection fails.
        /// </summary>
        public const string RomInjectionFailed = "ROM_INJECTION_FAILED";

        /// <summary>
        /// Error code for when metadata editing fails.
        /// </summary>
        public const string MetadataEditFailed = "METADATA_EDIT_FAILED";

        /// <summary>
        /// Error code for when boot sound write fails.
        /// </summary>
        public const string BootSoundWriteFailed = "BOOT_SOUND_WRITE_FAILED";

        /// <summary>
        /// Error code for when packaging fails.
        /// </summary>
        public const string PackagingFailed = "PACKAGING_FAILED";

        /// <summary>
        /// Error code for when there is insufficient disk space.
        /// </summary>
        public const string InsufficientDiskSpace = "INSUFFICIENT_DISK_SPACE";

        /// <summary>
        /// Error code for when the injection pipeline fails.
        /// </summary>
        public const string PipelineFailed = "PIPELINE_FAILED";

        /// <summary>
        /// Error code for when a title key is not found.
        /// </summary>
        public const string TitleKeyNotFound = "TITLE_KEY_NOT_FOUND";

        /// <summary>
        /// Error code for when storing a title key fails.
        /// </summary>
        public const string TitleKeyStoreFailed = "TITLE_KEY_STORE_FAILED";

        /// <summary>
        /// Error code for when settings cannot be loaded.
        /// </summary>
        public const string SettingsLoadFailed = "SETTINGS_LOAD_FAILED";

        /// <summary>
        /// Error code for when settings cannot be saved.
        /// </summary>
        public const string SettingsSaveFailed = "SETTINGS_SAVE_FAILED";

        /// <summary>
        /// Error code for when an update check fails.
        /// </summary>
        public const string UpdateCheckFailed = "UPDATE_CHECK_FAILED";

        /// <summary>
        /// Error code for when environment inspection fails.
        /// </summary>
        public const string EnvironmentInspectionFailed = "ENVIRONMENT_INSPECTION_FAILED";

        /// <summary>
        /// Error code for when GCT code parsing fails.
        /// </summary>
        public const string GctParseFailed = "GCT_PARSE_FAILED";

        /// <summary>
        /// Error code for when DOL patching fails.
        /// </summary>
        public const string DolPatchFailed = "DOL_PATCH_FAILED";

        /// <summary>
        /// Error code for when an operation is cancelled.
        /// </summary>
        public const string OperationCancelled = "OPERATION_CANCELLED";
    }
}
