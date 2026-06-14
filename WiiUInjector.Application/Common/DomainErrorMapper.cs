using System;
using WiiUInjector.Domain.Services;

namespace WiiUInjector.Application.Common
{
    /// <summary>
    /// Static utility class for mapping domain-layer validation errors to application-layer result objects.
    /// </summary>
    public static class DomainErrorMapper
    {
        /// <summary>
        /// Converts a domain ValidationError to an ApplicationError.
        /// </summary>
        /// <param name="domainError">The domain validation error to convert; must not be null.</param>
        /// <returns>An ApplicationError with the same code, message, and field.</returns>
        /// <exception cref="ArgumentNullException">Thrown if domainError is null.</exception>
        public static ApplicationError ToApplicationError(ValidationError domainError)
        {
            if (domainError == null)
                throw new ArgumentNullException(nameof(domainError));

            return new ApplicationError(domainError.Code, domainError.Message, domainError.Field);
        }

        /// <summary>
        /// Converts a domain ValidationResult to an application Result.
        /// </summary>
        /// <param name="validationResult">The domain validation result to convert; must not be null.</param>
        /// <returns>Result.Success() if valid; Result.Failure(...) with mapped errors otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if validationResult is null.</exception>
        public static Result ToResult(ValidationResult validationResult)
        {
            if (validationResult == null)
                throw new ArgumentNullException(nameof(validationResult));

            if (validationResult.IsValid)
                return Result.Success();

            var appErrors = new ApplicationError[validationResult.Errors.Count];
            for (int i = 0; i < validationResult.Errors.Count; i++)
                appErrors[i] = ToApplicationError(validationResult.Errors[i]);

            return Result.Failure(appErrors);
        }

        /// <summary>
        /// Converts a domain ValidationResult to a typed application Result&lt;T&gt;.
        /// </summary>
        /// <typeparam name="T">The type of the success value.</typeparam>
        /// <param name="validationResult">The domain validation result to convert; must not be null.</param>
        /// <param name="successValue">The value to return on success.</param>
        /// <returns>Result&lt;T&gt;.Success(successValue) if valid; Result&lt;T&gt;.Failure(...) with mapped errors otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown if validationResult is null.</exception>
        public static Result<T> ToResult<T>(ValidationResult validationResult, T successValue)
        {
            if (validationResult == null)
                throw new ArgumentNullException(nameof(validationResult));

            if (validationResult.IsValid)
                return Result<T>.Success(successValue);

            var appErrors = new ApplicationError[validationResult.Errors.Count];
            for (int i = 0; i < validationResult.Errors.Count; i++)
                appErrors[i] = ToApplicationError(validationResult.Errors[i]);

            return Result<T>.Failure(appErrors);
        }
    }
}
