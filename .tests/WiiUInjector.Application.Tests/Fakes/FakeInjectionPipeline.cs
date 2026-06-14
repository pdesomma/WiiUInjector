#nullable disable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WiiUInjector.Application.Common;
using WiiUInjector.Application.Services.Injections;
using WiiUInjector.Domain;

namespace WiiUInjector.Application.Tests.Fakes
{
    /// <summary>
    /// Test double for IInjectionPipeline with configurable execution result.
    /// </summary>
    public sealed class FakeInjectionPipeline : IInjectionPipeline
    {
        /// <summary>
        /// Result to return from RunAsync; defaults to a success outcome.
        /// </summary>
        public Result<InjectionOutcome> NextResult { get; set; }

        /// <summary>
        /// Exception to throw from RunAsync; when set, overrides NextResult.
        /// </summary>
        public Exception NextException { get; set; }

        /// <summary>
        /// Number of times RunAsync was called.
        /// </summary>
        public int CallCount { get; set; }

        /// <summary>
        /// The most recently received Injection.
        /// </summary>
        public Injection LastInjection { get; set; }

        /// <summary>
        /// Initializes FakeInjectionPipeline with a default success outcome.
        /// </summary>
        public FakeInjectionPipeline()
        {
            var defaultOutcome = new InjectionOutcome("test_package.wua", new List<string>(), TimeSpan.FromSeconds(1));
            NextResult = Result<InjectionOutcome>.Success(defaultOutcome);
        }

        /// <summary>
        /// Throws NextException if set; otherwise returns NextResult. Records call details.
        /// </summary>
        public Task<Result<InjectionOutcome>> RunAsync(
            Injection injection,
            InjectionOutputSpec output,
            IProgress<InjectionProgress> progress,
            CancellationToken cancellationToken)
        {
            if (injection == null)
                throw new ArgumentNullException(nameof(injection));

            CallCount++;
            LastInjection = injection;

            if (NextException != null)
                throw NextException;

            return Task.FromResult(NextResult);
        }
    }
}
