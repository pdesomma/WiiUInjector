#nullable disable
using System;
using System.Collections.Generic;
using WiiUInjector.Application.Abstractions.Logging;

namespace WiiUInjector.Application.Tests.Fakes
{
    /// <summary>
    /// Test double for IApplicationLogger that records all log messages.
    /// </summary>
    public sealed class FakeApplicationLogger : IApplicationLogger
    {
        /// <summary>
        /// List of all informational messages logged.
        /// </summary>
        public List<string> InfoMessages { get; } = new List<string>();

        /// <summary>
        /// List of all warning messages logged.
        /// </summary>
        public List<string> WarnMessages { get; } = new List<string>();

        /// <summary>
        /// List of all error messages with optional exceptions.
        /// </summary>
        public List<(string Message, Exception Ex)> ErrorMessages { get; } = new List<(string, Exception)>();

        /// <summary>
        /// Logs an informational message.
        /// </summary>
        public void Info(string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            InfoMessages.Add(message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        public void Warn(string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            WarnMessages.Add(message);
        }

        /// <summary>
        /// Logs an error message with optional exception.
        /// </summary>
        public void Error(string message, Exception exception = null)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            ErrorMessages.Add((message, exception));
        }
    }
}
