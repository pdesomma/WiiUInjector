using System.Collections.Generic;

namespace WiiUInjector.Domain.Services
{
    /// <summary>
    /// Parses .dol headers, translates addresses, applies GCT cheat patches and injects a codehandler — all in-memory.
    /// </summary>
    public interface IDolPatcher
    {
        /// <summary>
        /// Reads the section table from a raw .dol byte buffer. Returns sections with non-zero size only.
        /// </summary>
        IReadOnlyList<DolSection> ReadHeader(byte[] dolData);

        /// <summary>
        /// Translates a memory address to a file offset inside the .dol buffer, or -1 if the address falls outside every section.
        /// </summary>
        int MemoryToDolOffset(uint memoryAddress, IReadOnlyList<DolSection> sections);

        /// <summary>
        /// Returns a new buffer containing the original .dol with every GCT code applied as a big-endian uint32 overwrite at the translated file offset.
        /// </summary>
        /// <param name="dolData">Source .dol bytes; not modified.</param>
        /// <param name="codes">Codes to apply. Every code address MUST fall inside a section — throws if any does not.</param>
        byte[] ApplyCodes(byte[] dolData, IReadOnlyList<GctCode> codes);

        /// <summary>
        /// Returns a new buffer containing the original .dol with the codehandler injected at the first run of <paramref name="codehandler"/>.Length zero bytes, and the entry point at file offset 0xE0 rewritten to 0x80000000 + injectionOffset (big-endian). Throws if no free space is found.
        /// </summary>
        byte[] InjectCodehandler(byte[] dolData, byte[] codehandler);
    }
}
