using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;

namespace WiiUInjector.Domain.Services
{
    /// <summary>
    /// Parses GCT cheat codelists from binary .gct format and textual Ocarina / Dolphin formats.
    /// </summary>
    public sealed class GctParser : IGctParser
    {
        private static readonly Regex HexCodePattern = new Regex(@"^[0-9A-Fa-f]{8}\s+[0-9A-Fa-f]{8}$", RegexOptions.Compiled);

        /// <summary>
        /// Parses a binary .gct buffer. Stops at the standard terminator (address 0xF0000000, value 0x00000000) if present.
        /// </summary>
        /// <param name="gctData">The raw .gct bytes.</param>
        /// <returns>A read-only list of GCT codes.</returns>
        /// <exception cref="ArgumentNullException">Thrown when gctData is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the buffer is too small or its length is not a multiple of 8.</exception>
        public IReadOnlyList<GctCode> ParseBinary(byte[] gctData)
        {
            if (gctData == null)
                throw new ArgumentNullException(nameof(gctData));

            if (gctData.Length < 8)
                throw new ArgumentException("GCT buffer is too small.", nameof(gctData));

            if (gctData.Length % 8 != 0)
                throw new ArgumentException("GCT buffer length must be a multiple of 8 bytes.", nameof(gctData));

            var codes = new List<GctCode>();

            for (int i = 0; i + 8 <= gctData.Length; i += 8)
            {
                uint address = ReadUInt32BigEndian(gctData, i);
                uint value = ReadUInt32BigEndian(gctData, i + 4);

                if (address == 0xF0000000u && value == 0x00000000u)
                    break;

                codes.Add(new GctCode(address, value));
            }

            return new ReadOnlyCollection<GctCode>(codes);
        }

        /// <summary>
        /// Parses a textual codelist in Ocarina or Dolphin format. Both formats use "AAAAAAAA VVVVVVVV" per line; Dolphin uses [Gecko] section header.
        /// </summary>
        /// <param name="textContent">The text content to parse.</param>
        /// <returns>A read-only list of GCT codes.</returns>
        /// <exception cref="ArgumentNullException">Thrown when textContent is null.</exception>
        /// <exception cref="InvalidDataException">Thrown on malformed lines or when no codes are found.</exception>
        public IReadOnlyList<GctCode> ParseText(string textContent)
        {
            if (textContent == null)
                throw new ArgumentNullException(nameof(textContent));

            var codes = new List<GctCode>();
            bool insideGeckoSection = false;

            var lines = textContent.Split('\n');

            foreach (var line in lines)
            {
                var trimmed = line.Trim();

                if (string.IsNullOrEmpty(trimmed))
                    continue;

                if (trimmed.StartsWith("#", StringComparison.Ordinal))
                    continue;

                if (trimmed.StartsWith("$", StringComparison.Ordinal))
                    continue;

                if (trimmed.StartsWith("[", StringComparison.Ordinal) && trimmed.EndsWith("]", StringComparison.Ordinal))
                {
                    insideGeckoSection = trimmed.Equals("[Gecko]", StringComparison.OrdinalIgnoreCase);
                    continue;
                }

                if (!HexCodePattern.IsMatch(trimmed))
                {
                    if (insideGeckoSection)
                        continue;

                    throw new InvalidDataException($"Invalid line format: {trimmed}");
                }

                var parts = trimmed.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    throw new InvalidDataException($"Invalid line format: {trimmed}");

                uint address = Convert.ToUInt32(parts[0], 16);
                uint value = Convert.ToUInt32(parts[1], 16);

                codes.Add(new GctCode(address, value));
            }

            if (codes.Count == 0)
                throw new InvalidDataException("No valid cheat codes found.");

            return new ReadOnlyCollection<GctCode>(codes);
        }

        /// <summary>
        /// Reads a big-endian uint32 from the buffer at the specified offset.
        /// </summary>
        /// <param name="data">The buffer.</param>
        /// <param name="offset">The offset to read from.</param>
        /// <returns>The uint32 value in host endianness.</returns>
        private static uint ReadUInt32BigEndian(byte[] data, int offset)
        {
            return ((uint)data[offset] << 24)
                 | ((uint)data[offset + 1] << 16)
                 | ((uint)data[offset + 2] << 8)
                 |  data[offset + 3];
        }
    }
}
