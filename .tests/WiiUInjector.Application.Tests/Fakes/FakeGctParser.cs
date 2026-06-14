#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WiiUInjector.Domain;
using WiiUInjector.Domain.Services;

namespace WiiUInjector.Application.Tests.Fakes
{
    /// <summary>
    /// Test double for IGctParser with configurable parse results.
    /// </summary>
    public sealed class FakeGctParser : IGctParser
    {
        /// <summary>
        /// Codes to return from ParseBinary; defaults to a single test code.
        /// </summary>
        public IReadOnlyList<GctCode> NextBinaryCodes { get; set; }

        /// <summary>
        /// Codes to return from ParseText; defaults to a single test code.
        /// </summary>
        public IReadOnlyList<GctCode> NextTextCodes { get; set; }

        /// <summary>
        /// Exception to throw from ParseBinary; overrides NextBinaryCodes if set.
        /// </summary>
        public Exception BinaryException { get; set; }

        /// <summary>
        /// Exception to throw from ParseText; overrides NextTextCodes if set.
        /// </summary>
        public Exception TextException { get; set; }

        /// <summary>
        /// Number of times ParseBinary was called.
        /// </summary>
        public int ParseBinaryCallCount { get; set; }

        /// <summary>
        /// Number of times ParseText was called.
        /// </summary>
        public int ParseTextCallCount { get; set; }

        /// <summary>
        /// Initializes FakeGctParser with default test codes.
        /// </summary>
        public FakeGctParser()
        {
            NextBinaryCodes = new ReadOnlyCollection<GctCode>(
                new List<GctCode> { new GctCode(0x80000000, 0x00000001) });
            NextTextCodes = new ReadOnlyCollection<GctCode>(
                new List<GctCode> { new GctCode(0x80000000, 0x00000001) });
        }

        /// <summary>
        /// Throws BinaryException if set; otherwise returns NextBinaryCodes.
        /// </summary>
        public IReadOnlyList<GctCode> ParseBinary(byte[] gctData)
        {
            ParseBinaryCallCount++;

            if (BinaryException != null)
                throw BinaryException;

            return NextBinaryCodes;
        }

        /// <summary>
        /// Throws TextException if set; otherwise returns NextTextCodes.
        /// </summary>
        public IReadOnlyList<GctCode> ParseText(string textContent)
        {
            ParseTextCallCount++;

            if (TextException != null)
                throw TextException;

            return NextTextCodes;
        }
    }
}
