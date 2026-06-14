using System;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Services.Injections
{
    /// <summary>
    /// Sealed POCO carrying parameters needed to run an injection via the pipeline.
    /// </summary>
    public sealed class RunInjectionCommand
    {
        /// <summary>
        /// The injection specification (ROM, console, images, options, etc.).
        /// </summary>
        public Injection Injection { get; }

        /// <summary>
        /// The output configuration (directory, format, key source, etc.).
        /// </summary>
        public InjectionOutputSpec Output { get; }

        /// <summary>
        /// Whether to skip free-disk-space checks (legacy --spacebypass semantics).
        /// </summary>
        public bool Force { get; }

        /// <summary>
        /// Initializes a new RunInjectionCommand.
        /// </summary>
        /// <param name="injection">The injection specification; must not be null.</param>
        /// <param name="output">The output configuration; must not be null.</param>
        /// <param name="force">Whether to skip space checks.</param>
        /// <exception cref="ArgumentNullException">Thrown if injection or output is null.</exception>
        public RunInjectionCommand(Injection injection, InjectionOutputSpec output, bool force = false)
        {
            if (injection == null)
                throw new ArgumentNullException(nameof(injection));

            if (output == null)
                throw new ArgumentNullException(nameof(output));

            Injection = injection;
            Output = output;
            Force = force;
        }
    }
}
