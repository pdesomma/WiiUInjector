using System;

namespace WiiUInjector.Application.Services.Injections
{
    /// <summary>
    /// A sealed value object representing the current progress of an injection operation.
    /// </summary>
    public sealed class InjectionProgress
    {
        /// <summary>
        /// Gets the percentage completion of the injection operation, from 0 to 100 inclusive.
        /// </summary>
        public int PercentComplete { get; }

        /// <summary>
        /// Gets the human-readable label for the current stage of the injection process
        /// (e.g., "Copying base ROM", "Patching DOL").
        /// </summary>
        public string StageLabel { get; }

        /// <summary>
        /// Gets an optional detailed message providing additional context for the current stage.
        /// May be null or empty.
        /// </summary>
        public string DetailMessage { get; }

        /// <summary>
        /// Initializes a new InjectionProgress with the specified percent complete, stage label, and optional detail message.
        /// </summary>
        /// <param name="percentComplete">The percentage completion (0–100 inclusive).</param>
        /// <param name="stageLabel">The stage label; must not be null or whitespace.</param>
        /// <param name="detailMessage">An optional detail message; may be null or empty.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if percentComplete is less than 0 or greater than 100.</exception>
        /// <exception cref="ArgumentNullException">Thrown if stageLabel is null.</exception>
        /// <exception cref="ArgumentException">Thrown if stageLabel is empty or whitespace.</exception>
        public InjectionProgress(int percentComplete, string stageLabel, string detailMessage = null)
        {
            if (percentComplete < 0 || percentComplete > 100)
                throw new ArgumentOutOfRangeException(nameof(percentComplete), percentComplete, "Percent complete must be between 0 and 100.");
            if (stageLabel == null)
                throw new ArgumentNullException(nameof(stageLabel));
            if (string.IsNullOrWhiteSpace(stageLabel))
                throw new ArgumentException("Stage label cannot be empty or whitespace.", nameof(stageLabel));

            PercentComplete = percentComplete;
            StageLabel = stageLabel;
            DetailMessage = detailMessage;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current InjectionProgress.
        /// Compares PercentComplete, StageLabel, and DetailMessage.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>true if the specified object is equal to the current instance; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is InjectionProgress other))
                return false;

            return PercentComplete == other.PercentComplete
                && StageLabel == other.StageLabel
                && DetailMessage == other.DetailMessage;
        }

        /// <summary>
        /// Serves as the default hash function for this type.
        /// </summary>
        /// <returns>A hash code for the current instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + PercentComplete.GetHashCode();
                hash = hash * 31 + (StageLabel == null ? 0 : StageLabel.GetHashCode());
                hash = hash * 31 + (DetailMessage == null ? 0 : DetailMessage.GetHashCode());
                return hash;
            }
        }

        /// <summary>
        /// Determines whether two specified InjectionProgress instances are equal.
        /// </summary>
        /// <param name="left">The first InjectionProgress to compare.</param>
        /// <param name="right">The second InjectionProgress to compare.</param>
        /// <returns>true if left equals right; otherwise false.</returns>
        public static bool operator ==(InjectionProgress left, InjectionProgress right)
        {
            if (left is null)
                return right is null;
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two specified InjectionProgress instances are not equal.
        /// </summary>
        /// <param name="left">The first InjectionProgress to compare.</param>
        /// <param name="right">The second InjectionProgress to compare.</param>
        /// <returns>true if left does not equal right; otherwise false.</returns>
        public static bool operator !=(InjectionProgress left, InjectionProgress right)
        {
            return !(left == right);
        }
    }
}
