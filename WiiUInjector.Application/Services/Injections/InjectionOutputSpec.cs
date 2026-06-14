using System;

namespace WiiUInjector.Application.Services.Injections
{
    /// <summary>
    /// A sealed value object specifying the output configuration for an injection operation.
    /// </summary>
    public sealed class InjectionOutputSpec
    {
        /// <summary>
        /// Gets the target output directory path where the injection result will be written.
        /// </summary>
        public string OutputDirectory { get; }

        /// <summary>
        /// Gets the desired output format for the injection result.
        /// </summary>
        public InjectionOutputFormat Format { get; }

        /// <summary>
        /// Gets the source of the common key, either as a file path or environment variable reference.
        /// May be null when using Loadiine format (which does not require a common key).
        /// </summary>
        public string CommonKeySource { get; }

        /// <summary>
        /// Initializes a new InjectionOutputSpec with the specified output directory, format, and optional common key source.
        /// </summary>
        /// <param name="outputDirectory">The target output directory; must not be null or whitespace.</param>
        /// <param name="format">The desired output format.</param>
        /// <param name="commonKeySource">The common key source (path or env var reference); may be null. Required if format is Packed.</param>
        /// <exception cref="ArgumentNullException">Thrown if outputDirectory is null.</exception>
        /// <exception cref="ArgumentException">Thrown if outputDirectory is whitespace or if format is Packed and commonKeySource is null or whitespace.</exception>
        public InjectionOutputSpec(string outputDirectory, InjectionOutputFormat format, string commonKeySource = null)
        {
            if (outputDirectory == null)
                throw new ArgumentNullException(nameof(outputDirectory));
            if (string.IsNullOrWhiteSpace(outputDirectory))
                throw new ArgumentException("Output directory cannot be empty or whitespace.", nameof(outputDirectory));
            if (format == InjectionOutputFormat.Packed && string.IsNullOrWhiteSpace(commonKeySource))
                throw new ArgumentException("Common key source is required when format is Packed.", nameof(commonKeySource));

            OutputDirectory = outputDirectory;
            Format = format;
            CommonKeySource = commonKeySource;
        }
    }
}
