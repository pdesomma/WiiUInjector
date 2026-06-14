using System;

namespace WiiUInjector.Domain.Services
{
    /// <summary>
    /// Immutable value object representing a single validation rule failure.
    /// </summary>
    public sealed class ValidationError
    {
        /// <summary>
        /// Short stable identifier for this error type (e.g., "ICON_DIMENSIONS").
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// Human-readable description of the error.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Property path or field identifier where the error occurred (e.g., "Images[Icon].Width"); may be null for errors not tied to a specific field.
        /// </summary>
        public string Field { get; }

        /// <summary>
        /// Constructs a ValidationError with the specified code, message, and optional field.
        /// </summary>
        /// <param name="code">Short stable identifier; must not be null or whitespace.</param>
        /// <param name="message">Human-readable description; must not be null or whitespace.</param>
        /// <param name="field">Property path (may be null).</param>
        /// <exception cref="ArgumentNullException">Thrown if code or message is null.</exception>
        /// <exception cref="ArgumentException">Thrown if code or message is empty or whitespace.</exception>
        public ValidationError(string code, string message, string field = null)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be empty.", nameof(code));

            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty.", nameof(message));

            Code = code;
            Message = message;
            Field = field;
        }

        /// <summary>
        /// Structural equality on code, message, and field using ordinal string comparison.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is ValidationError other))
                return false;

            return string.Equals(Code, other.Code, StringComparison.Ordinal) &&
                   string.Equals(Message, other.Message, StringComparison.Ordinal) &&
                   string.Equals(Field, other.Field, StringComparison.Ordinal);
        }

        /// <summary>
        /// Hash code combining code, message, and field.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Code.GetHashCode();
                hash = hash * 31 + Message.GetHashCode();
                hash = hash * 31 + (Field?.GetHashCode() ?? 0);
                return hash;
            }
        }

        /// <summary>
        /// Structural equality operator.
        /// </summary>
        public static bool operator ==(ValidationError left, ValidationError right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Structural inequality operator.
        /// </summary>
        public static bool operator !=(ValidationError left, ValidationError right)
        {
            return !(left == right);
        }
    }
}
