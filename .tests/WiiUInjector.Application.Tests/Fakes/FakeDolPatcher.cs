#nullable disable
using System;
using System.Collections.Generic;
using WiiUInjector.Domain;
using WiiUInjector.Domain.Services;

namespace WiiUInjector.Application.Tests.Fakes
{
    /// <summary>
    /// Test double for IDolPatcher with configurable patch results.
    /// </summary>
    public sealed class FakeDolPatcher : IDolPatcher
    {
        /// <summary>
        /// Bytes to return from ApplyCodes; defaults to a modified copy of input.
        /// </summary>
        public byte[] ApplyCodesResult { get; set; }

        /// <summary>
        /// Bytes to return from InjectCodehandler; defaults to a modified copy of input.
        /// </summary>
        public byte[] InjectCodehandlerResult { get; set; }

        /// <summary>
        /// Exception to throw from ApplyCodes; overrides ApplyCodesResult if set.
        /// </summary>
        public Exception ApplyCodesException { get; set; }

        /// <summary>
        /// Exception to throw from InjectCodehandler; overrides InjectCodehandlerResult if set.
        /// </summary>
        public Exception InjectCodehandlerException { get; set; }

        /// <summary>
        /// Sections to return from ReadHeader; defaults to an empty list.
        /// </summary>
        public IReadOnlyList<DolSection> ReadHeaderResult { get; set; }

        /// <summary>
        /// Number of times ApplyCodes was called.
        /// </summary>
        public int ApplyCodesCallCount { get; set; }

        /// <summary>
        /// Number of times InjectCodehandler was called.
        /// </summary>
        public int InjectCodehandlerCallCount { get; set; }

        /// <summary>
        /// Number of times ReadHeader was called.
        /// </summary>
        public int ReadHeaderCallCount { get; set; }

        /// <summary>
        /// Most recently received DOL bytes for ApplyCodes.
        /// </summary>
        public byte[] LastApplyCodesDol { get; set; }

        /// <summary>
        /// Most recently received DOL bytes for InjectCodehandler.
        /// </summary>
        public byte[] LastInjectCodehandlerDol { get; set; }

        /// <summary>
        /// Initializes FakeDolPatcher with default results.
        /// </summary>
        public FakeDolPatcher()
        {
            ApplyCodesResult = new byte[] { 0x00 };
            InjectCodehandlerResult = new byte[] { 0x00 };
            ReadHeaderResult = new List<DolSection>();
        }

        /// <summary>
        /// Throws ApplyCodesException if set; otherwise returns ApplyCodesResult.
        /// </summary>
        public byte[] ApplyCodes(byte[] dolData, IReadOnlyList<GctCode> codes)
        {
            ApplyCodesCallCount++;
            LastApplyCodesDol = dolData;

            if (ApplyCodesException != null)
                throw ApplyCodesException;

            return ApplyCodesResult;
        }

        /// <summary>
        /// Throws InjectCodehandlerException if set; otherwise returns InjectCodehandlerResult.
        /// </summary>
        public byte[] InjectCodehandler(byte[] dolData, byte[] codehandler)
        {
            InjectCodehandlerCallCount++;
            LastInjectCodehandlerDol = dolData;

            if (InjectCodehandlerException != null)
                throw InjectCodehandlerException;

            return InjectCodehandlerResult;
        }

        /// <summary>
        /// Returns the configured ReadHeaderResult.
        /// </summary>
        public IReadOnlyList<DolSection> ReadHeader(byte[] dolData)
        {
            ReadHeaderCallCount++;
            return ReadHeaderResult;
        }

        /// <summary>
        /// Returns -1 as a stub implementation.
        /// </summary>
        public int MemoryToDolOffset(uint memoryAddress, IReadOnlyList<DolSection> sections)
        {
            return -1;
        }
    }
}
