using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WiiUInjector.Application.Common
{
    /// <summary>
    /// Immutable value object representing the outcome of an operation (success or failure with errors).
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Whether the operation succeeded (no errors).
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Whether the operation failed (has errors).
        /// </summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// Readonly list of all application errors; empty if operation succeeded.
        /// </summary>
        public IReadOnlyList<ApplicationError> Errors { get; }

        /// <summary>
        /// Constructs a Result with the specified error list.
        /// </summary>
        /// <param name="errors">List of application errors; must not be null (use empty list for success).</param>
        /// <exception cref="ArgumentNullException">Thrown if errors is null.</exception>
        protected Result(IReadOnlyList<ApplicationError> errors)
        {
            if (errors == null)
                throw new ArgumentNullException(nameof(errors));

            var copy = new List<ApplicationError>(errors.Count);
            foreach (var err in errors)
                copy.Add(err);

            Errors = new ReadOnlyCollection<ApplicationError>(copy);
            IsSuccess = (Errors.Count == 0);
        }

        /// <summary>
        /// Returns a singleton success result (no errors).
        /// </summary>
        public static Result Success()
        {
            return new Result(new ApplicationError[0]);
        }

        /// <summary>
        /// Creates a failure result with the specified errors.
        /// </summary>
        /// <param name="errors">One or more application errors; must not be null or empty.</param>
        /// <returns>A failed Result containing the specified errors.</returns>
        /// <exception cref="ArgumentException">Thrown if errors is empty or contains null.</exception>
        public static Result Failure(params ApplicationError[] errors)
        {
            if (errors == null || errors.Length == 0)
                throw new ArgumentException("At least one error must be provided.", nameof(errors));

            foreach (var error in errors)
            {
                if (error == null)
                    throw new ArgumentException("Error list cannot contain null elements.", nameof(errors));
            }

            return new Result(errors);
        }

        /// <summary>
        /// Creates a failure result with the specified errors.
        /// </summary>
        /// <param name="errors">List of application errors; must not be null or empty.</param>
        /// <returns>A failed Result containing the specified errors.</returns>
        /// <exception cref="ArgumentException">Thrown if errors is empty or contains null.</exception>
        public static Result Failure(IReadOnlyList<ApplicationError> errors)
        {
            if (errors == null || errors.Count == 0)
                throw new ArgumentException("At least one error must be provided.", nameof(errors));

            foreach (var error in errors)
            {
                if (error == null)
                    throw new ArgumentException("Error list cannot contain null elements.", nameof(errors));
            }

            return new Result(errors);
        }
    }

    /// <summary>
    /// Generic immutable value object representing the outcome of an operation that produces a value on success.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    public sealed class Result<T> : Result
    {
        /// <summary>
        /// The success value; meaningful only when IsSuccess is true. Returns default(T) on failure.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Constructs a success result with the specified value.
        /// </summary>
        /// <param name="value">The success value (may be null if T is a nullable type).</param>
        private Result(T value)
            : base(new ApplicationError[0])
        {
            Value = value;
        }

        /// <summary>
        /// Constructs a failure result with the specified errors.
        /// </summary>
        /// <param name="errors">List of application errors; must not be null.</param>
        private Result(IReadOnlyList<ApplicationError> errors)
            : base(errors)
        {
            Value = default(T);
        }

        /// <summary>
        /// Creates a success result with the specified value.
        /// </summary>
        /// <param name="value">The success value (may be null if T is a nullable type).</param>
        /// <returns>A successful Result&lt;T&gt; containing the specified value.</returns>
        public static Result<T> Success(T value)
        {
            return new Result<T>(value);
        }

        /// <summary>
        /// Creates a failure result with the specified errors.
        /// </summary>
        /// <param name="errors">One or more application errors; must not be null or empty.</param>
        /// <returns>A failed Result&lt;T&gt; containing the specified errors.</returns>
        /// <exception cref="ArgumentException">Thrown if errors is empty or contains null.</exception>
        public static new Result<T> Failure(params ApplicationError[] errors)
        {
            if (errors == null || errors.Length == 0)
                throw new ArgumentException("At least one error must be provided.", nameof(errors));

            foreach (var error in errors)
            {
                if (error == null)
                    throw new ArgumentException("Error list cannot contain null elements.", nameof(errors));
            }

            return new Result<T>(errors);
        }

        /// <summary>
        /// Creates a failure result with the specified errors.
        /// </summary>
        /// <param name="errors">List of application errors; must not be null or empty.</param>
        /// <returns>A failed Result&lt;T&gt; containing the specified errors.</returns>
        /// <exception cref="ArgumentException">Thrown if errors is empty or contains null.</exception>
        public static new Result<T> Failure(IReadOnlyList<ApplicationError> errors)
        {
            if (errors == null || errors.Count == 0)
                throw new ArgumentException("At least one error must be provided.", nameof(errors));

            foreach (var error in errors)
            {
                if (error == null)
                    throw new ArgumentException("Error list cannot contain null elements.", nameof(errors));
            }

            return new Result<T>(errors);
        }

        /// <summary>
        /// Creates a failure Result&lt;T&gt; from an existing failed Result.
        /// Convenience method to re-shape a non-generic failure result into a typed one.
        /// </summary>
        /// <param name="other">A failed Result (IsSuccess must be false).</param>
        /// <returns>A failed Result&lt;T&gt; with the same errors.</returns>
        /// <exception cref="ArgumentException">Thrown if other.IsSuccess is true.</exception>
        public static Result<T> Failure(Result other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            if (other.IsSuccess)
                throw new ArgumentException("Cannot create a failure Result<T> from a successful Result.", nameof(other));

            return new Result<T>(other.Errors);
        }
    }
}
