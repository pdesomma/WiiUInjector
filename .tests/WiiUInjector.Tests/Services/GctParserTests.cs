#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Domain;
using WiiUInjector.Domain.Services;

namespace WiiUInjector.Tests.Services
{
    [TestClass]
    public class GctParserTests
    {
        private GctParser sut;

        [TestInitialize]
        public void Setup() => sut = new GctParser();

        /// <summary>
        /// ParseBinary throws ArgumentNullException when gctData is null.
        /// </summary>
        [TestMethod]
        public void ParseBinary_NullData_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => sut.ParseBinary(null));
        }

        /// <summary>
        /// ParseBinary throws ArgumentException when buffer is smaller than 8 bytes.
        /// </summary>
        [TestMethod]
        public void ParseBinary_TooSmall_Throws()
        {
            var buffer = new byte[4];
            Assert.ThrowsException<ArgumentException>(() => sut.ParseBinary(buffer));
        }

        /// <summary>
        /// ParseBinary throws ArgumentException when buffer length is not a multiple of 8.
        /// </summary>
        [TestMethod]
        public void ParseBinary_NotMultipleOfEight_Throws()
        {
            var buffer = new byte[9];
            Assert.ThrowsException<ArgumentException>(() => sut.ParseBinary(buffer));
        }

        /// <summary>
        /// ParseBinary parses single code from 8-byte buffer without terminator.
        /// </summary>
        [TestMethod]
        public void ParseBinary_OneCodeNoTerminator_ParsesOne()
        {
            var buffer = new byte[] { 0x00, 0x00, 0x12, 0x34, 0xAB, 0xCD, 0xEF, 0x01 };
            var codes = sut.ParseBinary(buffer);

            Assert.AreEqual(1, codes.Count);
            Assert.AreEqual(0x00001234u, codes[0].Address);
            Assert.AreEqual(0xABCDEF01u, codes[0].Value);
        }

        /// <summary>
        /// ParseBinary stops parsing at standard terminator (0xF0000000 0x00000000).
        /// </summary>
        [TestMethod]
        public void ParseBinary_StopsAtTerminator()
        {
            var buffer = new byte[24];
            WriteBE(buffer, 0, 0x04A12B34u);
            WriteBE(buffer, 4, 0xDEADBEEFu);
            WriteBE(buffer, 8, 0xF0000000u);
            WriteBE(buffer, 12, 0x00000000u);
            WriteBE(buffer, 16, 0x08000000u);
            WriteBE(buffer, 20, 0x12345678u);

            var codes = sut.ParseBinary(buffer);

            Assert.AreEqual(1, codes.Count);
            Assert.AreEqual(0x04A12B34u, codes[0].Address);
        }

        /// <summary>
        /// ParseText throws ArgumentNullException when textContent is null.
        /// </summary>
        [TestMethod]
        public void ParseText_NullContent_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => sut.ParseText(null));
        }

        /// <summary>
        /// ParseText throws InvalidDataException when no codes are found.
        /// </summary>
        [TestMethod]
        public void ParseText_EmptyContent_Throws()
        {
            Assert.ThrowsException<InvalidDataException>(() => sut.ParseText(""));
        }

        /// <summary>
        /// ParseText parses single valid hex line.
        /// </summary>
        [TestMethod]
        public void ParseText_SingleValidLine_ParsesOne()
        {
            var codes = sut.ParseText("04A12B34 DEADBEEF");

            Assert.AreEqual(1, codes.Count);
            Assert.AreEqual(0x04A12B34u, codes[0].Address);
            Assert.AreEqual(0xDEADBEEFu, codes[0].Value);
        }

        /// <summary>
        /// ParseText skips comment lines and blank lines.
        /// </summary>
        [TestMethod]
        public void ParseText_SkipsCommentsAndBlankLines()
        {
            var input = "# comment\n\n04A12B34 DEADBEEF";
            var codes = sut.ParseText(input);

            Assert.AreEqual(1, codes.Count);
            Assert.AreEqual(0x04A12B34u, codes[0].Address);
        }

        /// <summary>
        /// ParseText parses Ocarina codes with game ID header and code name.
        /// </summary>
        [TestMethod]
        public void ParseText_OcarinaWithGameIdHeader_ParsesCodes()
        {
            var input = "[SXXE01]\n$Infinite Lives\n04A12B34 DEADBEEF";
            var codes = sut.ParseText(input);

            Assert.AreEqual(1, codes.Count);
            Assert.AreEqual(0x04A12B34u, codes[0].Address);
        }

        /// <summary>
        /// ParseText silently skips metadata lines inside [Gecko] section.
        /// </summary>
        [TestMethod]
        public void ParseText_DolphinGeckoSection_SilentlySkipsMetadata()
        {
            var input = "[Gecko]\n$Some Code [Author]\n04A12B34 DEADBEEF\nRandom metadata line that does not match hex";
            var codes = sut.ParseText(input);

            Assert.AreEqual(1, codes.Count);
            Assert.AreEqual(0x04A12B34u, codes[0].Address);
        }

        /// <summary>
        /// ParseText throws InvalidDataException on malformed line outside [Gecko] section.
        /// </summary>
        [TestMethod]
        public void ParseText_MalformedLineOutsideGecko_Throws()
        {
            var input = "GARBAGE LINE\n04A12B34 DEADBEEF";
            Assert.ThrowsException<InvalidDataException>(() => sut.ParseText(input));
        }

        private static void WriteBE(byte[] data, int offset, uint value)
        {
            data[offset] = (byte)((value >> 24) & 0xFF);
            data[offset + 1] = (byte)((value >> 16) & 0xFF);
            data[offset + 2] = (byte)((value >> 8) & 0xFF);
            data[offset + 3] = (byte)(value & 0xFF);
        }
    }
}
