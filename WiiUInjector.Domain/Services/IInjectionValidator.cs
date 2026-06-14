namespace WiiUInjector.Domain.Services
{
    /// <summary>
    /// Validates business rules on an Injection that the aggregate's constructor invariants don't enforce (dimensions, sizes, option combos).
    /// </summary>
    public interface IInjectionValidator
    {
        /// <summary>
        /// Runs all business rules and returns the accumulated errors. Never throws on rule failure.
        /// </summary>
        /// <param name="injection">The injection to validate.</param>
        /// <returns>A ValidationResult containing any rule violations; IsValid is true if no errors.</returns>
        ValidationResult Validate(Injection injection);
    }
}
