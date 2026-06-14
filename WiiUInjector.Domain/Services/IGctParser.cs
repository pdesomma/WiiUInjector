using System.Collections.Generic;

namespace WiiUInjector.Domain.Services
{
    /// <summary>
    /// Parses GCT cheat codelists from the binary .gct format and from textual Ocarina / Dolphin codelists.
    /// </summary>
    public interface IGctParser
    {
        /// <summary>
        /// Parses a binary .gct buffer. Stops at the standard terminator (address 0xF0000000, value 0x00000000) if present. Throws if the buffer is null or its non-header length is not a multiple of 8.
        /// </summary>
        IReadOnlyList<GctCode> ParseBinary(byte[] gctData);
        /// <summary>
        /// Parses a textual codelist in Ocarina or Dolphin format. Both share the same line format ("AAAAAAAA VVVVVVVV"); the Dolphin variant has an `[Gecko]` section header, the Ocarina variant uses a `[GAMEID]` header. Throws InvalidDataException on malformed lines and when no codes are found.
        /// </summary>
        IReadOnlyList<GctCode> ParseText(string textContent);
    }
}