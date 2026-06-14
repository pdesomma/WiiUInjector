#nullable disable
using System;
using WiiUInjector.Domain;
using WiiUInjector.Domain.Services;

namespace WiiUInjector.Application.Tests.Fakes
{
    /// <summary>
    /// Test double for IInjectionValidator with configurable validation result.
    /// </summary>
    public sealed class FakeInjectionValidator : IInjectionValidator
    {
        /// <summary>
        /// The ValidationResult to return from Validate; defaults to success.
        /// </summary>
        public ValidationResult NextResult { get; set; }

        /// <summary>
        /// Number of times Validate was called.
        /// </summary>
        public int CallCount { get; set; }

        /// <summary>
        /// The most recently validated Injection instance.
        /// </summary>
        public Injection LastInjection { get; set; }

        /// <summary>
        /// Initializes FakeInjectionValidator with a success result.
        /// </summary>
        public FakeInjectionValidator()
        {
            NextResult = ValidationResult.Success();
        }

        /// <summary>
        /// Returns the configured NextResult; records call count and injection.
        /// </summary>
        public ValidationResult Validate(Injection injection)
        {
            if (injection == null)
                throw new ArgumentNullException(nameof(injection));

            CallCount++;
            LastInjection = injection;
            return NextResult;
        }
    }
}
