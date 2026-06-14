using System;
using System.Collections.Generic;

namespace WiiUInjector.Domain.Services
{
    /// <summary>
    /// Parses .dol headers, translates addresses, applies GCT cheat patches and injects a codehandler — all in-memory.
    /// </summary>
    public sealed class DolPatcher : IDolPatcher
    {
        /// <summary>
        /// Reads the section table from a raw .dol byte buffer. Returns sections with non-zero size only.
        /// </summary>
        /// <param name="dolData">The raw .dol bytes.</param>
        /// <returns>A read-only list of sections with non-zero size.</returns>
        /// <exception cref="ArgumentNullException">Thrown when dolData is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the buffer is too small to be a valid .dol.</exception>
        public IReadOnlyList<DolSection> ReadHeader(byte[] dolData)
        {
            if (dolData == null)
                throw new ArgumentNullException(nameof(dolData));

            if (dolData.Length < 0x100)
                throw new ArgumentException("Buffer too small to be a valid .dol.", nameof(dolData));

            var offsets = new uint[18];
            var addrs = new uint[18];
            var sizes = new uint[18];

            for (int i = 0; i < 18; i++)
                offsets[i] = ReadUInt32BigEndian(dolData, i * 4);

            for (int i = 0; i < 18; i++)
                addrs[i] = ReadUInt32BigEndian(dolData, 0x48 + (i * 4));

            for (int i = 0; i < 18; i++)
                sizes[i] = ReadUInt32BigEndian(dolData, 0x90 + (i * 4));

            var result = new List<DolSection>();
            for (int i = 0; i < 18; i++)
            {
                if (sizes[i] != 0)
                    result.Add(new DolSection(addrs[i], offsets[i], sizes[i]));
            }

            return result;
        }

        /// <summary>
        /// Translates a memory address to a file offset inside the .dol buffer, or -1 if the address falls outside every section.
        /// </summary>
        /// <param name="memoryAddress">The memory address to translate.</param>
        /// <param name="sections">The sections list from ReadHeader.</param>
        /// <returns>The file offset, or -1 if not found in any section.</returns>
        /// <exception cref="ArgumentNullException">Thrown when sections is null.</exception>
        /// <exception cref="ArgumentException">Thrown when sections is empty.</exception>
        public int MemoryToDolOffset(uint memoryAddress, IReadOnlyList<DolSection> sections)
        {
            if (sections == null)
                throw new ArgumentNullException(nameof(sections));

            if (sections.Count == 0)
                throw new ArgumentException("Sections list cannot be empty.", nameof(sections));

            foreach (var section in sections)
            {
                if (memoryAddress >= section.MemoryAddress && memoryAddress < section.MemoryAddress + section.Size)
                    return (int)(section.FileOffset + (memoryAddress - section.MemoryAddress));
            }

            return -1;
        }

        /// <summary>
        /// Returns a new buffer containing the original .dol with every GCT code applied as a big-endian uint32 overwrite at the translated file offset.
        /// </summary>
        /// <param name="dolData">Source .dol bytes; not modified.</param>
        /// <param name="codes">Codes to apply. Every code address MUST fall inside a section.</param>
        /// <returns>A new byte array with codes applied.</returns>
        /// <exception cref="ArgumentNullException">Thrown when dolData or codes is null.</exception>
        /// <exception cref="ArgumentException">Thrown when a code address is not within any section or would write past the buffer end.</exception>
        public byte[] ApplyCodes(byte[] dolData, IReadOnlyList<GctCode> codes)
        {
            if (dolData == null)
                throw new ArgumentNullException(nameof(dolData));

            if (codes == null)
                throw new ArgumentNullException(nameof(codes));

            var sections = ReadHeader(dolData);
            var output = (byte[])dolData.Clone();

            foreach (var code in codes)
            {
                int offset = MemoryToDolOffset(code.Address, sections);

                if (offset < 0)
                    throw new ArgumentException($"Code address 0x{code.Address:X8} is not within any section.", nameof(codes));

                if (offset + 4 > output.Length)
                    throw new ArgumentException($"Code address 0x{code.Address:X8} translates to offset {offset} which would write past end of buffer.", nameof(codes));

                WriteUInt32BigEndian(output, offset, code.Value);
            }

            return output;
        }

        /// <summary>
        /// Returns a new buffer containing the original .dol with the codehandler injected at the first run of codehandler.Length zero bytes, and the entry point at file offset 0xE0 rewritten to 0x80000000 + injectionOffset (big-endian).
        /// </summary>
        /// <param name="dolData">Source .dol bytes; not modified.</param>
        /// <param name="codehandler">The codehandler bytes to inject.</param>
        /// <returns>A new byte array with the codehandler injected.</returns>
        /// <exception cref="ArgumentNullException">Thrown when dolData or codehandler is null.</exception>
        /// <exception cref="ArgumentException">Thrown when codehandler is empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no free space is available for injection.</exception>
        public byte[] InjectCodehandler(byte[] dolData, byte[] codehandler)
        {
            if (dolData == null)
                throw new ArgumentNullException(nameof(dolData));

            if (codehandler == null)
                throw new ArgumentNullException(nameof(codehandler));

            if (codehandler.Length == 0)
                throw new ArgumentException("Codehandler cannot be empty.", nameof(codehandler));

            int injectionOffset = FindFreeSpace(dolData, codehandler.Length);

            if (injectionOffset < 0)
                throw new InvalidOperationException("No free space available for codehandler injection.");

            var output = (byte[])dolData.Clone();

            Array.Copy(codehandler, 0, output, injectionOffset, codehandler.Length);

            uint newEntryPoint = (uint)(0x80000000u + (uint)injectionOffset);
            WriteUInt32BigEndian(output, 0xE0, newEntryPoint);

            return output;
        }

        /// <summary>
        /// Reads a big-endian uint32 from the buffer at the specified offset.
        /// </summary>
        /// <param name="data">The buffer.</param>
        /// <param name="offset">The offset to read from.</param>
        /// <returns>The uint32 value.</returns>
        private static uint ReadUInt32BigEndian(byte[] data, int offset)
        {
            return ((uint)data[offset] << 24)
                 | ((uint)data[offset + 1] << 16)
                 | ((uint)data[offset + 2] << 8)
                 |  (uint)data[offset + 3];
        }

        /// <summary>
        /// Writes a big-endian uint32 to the buffer at the specified offset.
        /// </summary>
        /// <param name="data">The buffer.</param>
        /// <param name="offset">The offset to write to.</param>
        /// <param name="value">The uint32 value to write.</param>
        private static void WriteUInt32BigEndian(byte[] data, int offset, uint value)
        {
            data[offset]     = (byte)((value >> 24) & 0xFF);
            data[offset + 1] = (byte)((value >> 16) & 0xFF);
            data[offset + 2] = (byte)((value >> 8) & 0xFF);
            data[offset + 3] = (byte) (value & 0xFF);
        }

        /// <summary>
        /// Finds the first run of requiredSize consecutive zero bytes in the buffer.
        /// </summary>
        /// <param name="data">The buffer to search.</param>
        /// <param name="requiredSize">The number of consecutive zero bytes required.</param>
        /// <returns>The offset of the first run, or -1 if not found.</returns>
        private static int FindFreeSpace(byte[] data, int requiredSize)
        {
            for (int i = 0; i <= data.Length - requiredSize; i++)
            {
                bool foundRun = true;
                for (int j = 0; j < requiredSize; j++)
                {
                    if (data[i + j] != 0)
                    {
                        foundRun = false;
                        break;
                    }
                }

                if (foundRun)
                    return i;
            }

            return -1;
        }
    }
}
