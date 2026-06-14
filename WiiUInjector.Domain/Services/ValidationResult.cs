using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WiiUInjector.Domain.Services
{
    /// <summary>
    /// Immutable value object representing the outcome of a validation run.
    /// </summary>
    public sealed class ValidationResult
    {
        /// <summary>
        /// Readonly list of all validation errors; empty if validation passed.
        /// </summary>
        public IReadOnlyList<ValidationError> Errors { get; }

        /// <summary>
        /// Whether validation passed (no errors).
        /// </summary>
        public bool IsValid { get; }

        /// <summary>
        /// Constructs a ValidationResult with the specified error list.
        /// </summary>
        /// <param name="errors">List of validation errors; must not be null (use empty list for success).</param>
        /// <exception cref="ArgumentNullException">Thrown if errors is null.</exception>
        public ValidationResult(IReadOnlyList<ValidationError> errors)
        {
            if (errors == null)
                throw new ArgumentNullException(nameof(errors));

            var copy = new List<ValidationError>(errors.Count);
            foreach (var err in errors)
                copy.Add(err);

            Errors = new ReadOnlyCollection<ValidationError>(copy);
            IsValid = (Errors.Count == 0);
        }

        /// <summary>
        /// Returns a singleton success result (no errors).
        /// </summary>
        public static ValidationResult Success()
        {
            return new ValidationResult(new ValidationError[0]);
        }
    }
}
