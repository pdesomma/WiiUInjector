using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WiiUInjector.Application.Services.Injections
{
    /// <summary>
    /// A sealed value object representing the result of a successful injection operation.
    /// </summary>
    public sealed class InjectionOutcome
    {
        /// <summary>
        /// Gets the final output path (file or directory) of the completed injection.
        /// </summary>
        public string OutputPath { get; }

        /// <summary>
        /// Gets a read-only collection of warning messages generated during the injection process.
        /// </summary>
        public IReadOnlyList<string> Warnings { get; }

        /// <summary>
        /// Gets the total duration of the injection operation.
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// Initializes a new InjectionOutcome with the specified output path, warnings, and duration.
        /// </summary>
        /// <param name="outputPath">The final output path; must not be null or whitespace.</param>
        /// <param name="warnings">A list of warning messages; must not be null (defensive copy will be made).</param>
        /// <param name="duration">The total duration of the injection operation.</param>
        /// <exception cref="ArgumentNullException">Thrown if outputPath or warnings is null.</exception>
        /// <exception cref="ArgumentException">Thrown if outputPath is empty or whitespace.</exception>
        public InjectionOutcome(string outputPath, IList<string> warnings, TimeSpan duration)
        {
            if (outputPath == null)
                throw new ArgumentNullException(nameof(outputPath));
            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Output path cannot be empty or whitespace.", nameof(outputPath));
            if (warnings == null)
                throw new ArgumentNullException(nameof(warnings));

            OutputPath = outputPath;
            Warnings = new ReadOnlyCollection<string>(warnings);
            Duration = duration;
        }
    }
}
