using System;

namespace WiiUInjector.Application.Abstractions.Logging
{
    /// <summary>
    /// Interface for application-wide logging, replacing legacy Logger.Log calls.
    /// </summary>
    public interface IApplicationLogger
    {
        /// <summary>
        /// Logs an informational message.
        /// </summary>
        /// <param name="message">The message to log; must not be null.</param>
        void Info(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message to log; must not be null.</param>
        void Warn(string message);

        /// <summary>
        /// Logs an error message with an optional exception.
        /// </summary>
        /// <param name="message">The message to log; must not be null.</param>
        /// <param name="exception">An optional exception associated with the error; may be null.</param>
        void Error(string message, Exception exception = null);
    }
}
