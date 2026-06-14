#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Domain;
using WiiUInjector.Domain.Services;

namespace WiiUInjector.Tests.Services
{
    [TestClass]
    public class DolPatcherTests
    {
        private DolPatcher sut;

        [TestInitialize]
        public void Setup() => sut = new DolPatcher();

        /// <summary>
        /// ReadHeader throws ArgumentNullException when dolData is null.
        /// </summary>
        [TestMethod]
        public void ReadHeader_NullData_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => sut.ReadHeader(null));
        }

        /// <summary>
        /// ReadHeader throws ArgumentException when buffer is smaller than 0x100 bytes.
        /// </summary>
        [TestMethod]
        public void ReadHeader_BufferTooSmall_Throws()
        {
            var buffer = new byte[0xFF];
            Assert.ThrowsException<ArgumentException>(() => sut.ReadHeader(buffer));
        }

        /// <summary>
        /// ReadHeader returns only sections with non-zero size.
        /// </summary>
        [TestMethod]
        public void ReadHeader_ValidHeader_ReturnsOnlyNonZeroSections()
        {
            var dol = new byte[0x100];
            WriteBE(dol, 0x00, 0x100u);
            WriteBE(dol, 0x08, 0x300u);
            WriteBE(dol, 0x48, 0x80003100u);
            WriteBE(dol, 0x50, 0x80004000u);
            WriteBE(dol, 0x90, 0x100u);
            WriteBE(dol, 0x98, 0x200u);

            var sections = sut.ReadHeader(dol);

            Assert.AreEqual(2, sections.Count);
            Assert.AreEqual(0x80003100u, sections[0].MemoryAddress);
            Assert.AreEqual(0x100u, sections[0].FileOffset);
            Assert.AreEqual(0x100u, sections[0].Size);
            Assert.AreEqual(0x80004000u, sections[1].MemoryAddress);
            Assert.AreEqual(0x300u, sections[1].FileOffset);
            Assert.AreEqual(0x200u, sections[1].Size);
        }

        /// <summary>
        /// MemoryToDolOffset returns translated offset for address inside a section.
        /// </summary>
        [TestMethod]
        public void MemoryToDolOffset_AddressInsideSection_ReturnsTranslatedOffset()
        {
            var sections = new List<DolSection> { new DolSection(0x80003100u, 0x100u, 0x200u) };
            var offset = sut.MemoryToDolOffset(0x80003110u, sections);
            Assert.AreEqual(0x110, offset);
        }

        /// <summary>
        /// MemoryToDolOffset returns -1 for address outside any section.
        /// </summary>
        [TestMethod]
        public void MemoryToDolOffset_AddressOutsideAnySection_ReturnsMinusOne()
        {
            var sections = new List<DolSection> { new DolSection(0x80003100u, 0x100u, 0x200u) };
            var offset = sut.MemoryToDolOffset(0x80009000u, sections);
            Assert.AreEqual(-1, offset);
        }

        /// <summary>
        /// MemoryToDolOffset throws ArgumentNullException when sections is null.
        /// </summary>
        [TestMethod]
        public void MemoryToDolOffset_NullSections_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => sut.MemoryToDolOffset(0x80003100u, null));
        }

        /// <summary>
        /// MemoryToDolOffset throws ArgumentException when sections list is empty.
        /// </summary>
        [TestMethod]
        public void MemoryToDolOffset_EmptySections_Throws()
        {
            var sections = new List<DolSection>();
            Assert.ThrowsException<ArgumentException>(() => sut.MemoryToDolOffset(0x80003100u, sections));
        }

        /// <summary>
        /// ApplyCodes writes code value as big-endian uint32 and returns defensive copy.
        /// </summary>
        [TestMethod]
        public void ApplyCodes_WritesValueAsBigEndian()
        {
            var dol = BuildMinimalDol(0x100u, 0x80003100u, 0x200u);
            var codes = new List<GctCode> { new GctCode(0x80003104u, 0xDEADBEEFu) };

            var result = sut.ApplyCodes(dol, codes);

            Assert.AreNotSame(dol, result);
            Assert.AreEqual(0xDE, result[0x104]);
            Assert.AreEqual(0xAD, result[0x105]);
            Assert.AreEqual(0xBE, result[0x106]);
            Assert.AreEqual(0xEF, result[0x107]);
        }

        /// <summary>
        /// ApplyCodes throws ArgumentException when code address is outside any section.
        /// </summary>
        [TestMethod]
        public void ApplyCodes_AddressOutsideSection_Throws()
        {
            var dol = BuildMinimalDol(0x100u, 0x80003100u, 0x200u);
            var codes = new List<GctCode> { new GctCode(0x80009000u, 0xDEADBEEFu) };

            Assert.ThrowsException<ArgumentException>(() => sut.ApplyCodes(dol, codes));
        }

        /// <summary>
        /// InjectCodehandler finds free space, injects codehandler, and rewrites entry point.
        /// </summary>
        [TestMethod]
        public void InjectCodehandler_FindsFreeSpaceAndWritesEntryPoint()
        {
            var dol = new byte[0x400];
            for (int i = 0; i <= 0x10; i++) dol[i] = 0xFF;
            var codehandler = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            var result = sut.InjectCodehandler(dol, codehandler);

            var injected = new byte[8];
            Array.Copy(result, 0x11, injected, 0, 8);
            CollectionAssert.AreEqual(codehandler, injected);
            Assert.AreEqual(0x80, result[0xE0]);
            Assert.AreEqual(0x00, result[0xE1]);
            Assert.AreEqual(0x00, result[0xE2]);
            Assert.AreEqual(0x11, result[0xE3]);
        }

        /// <summary>
        /// InjectCodehandler throws InvalidOperationException when no free space is available.
        /// </summary>
        [TestMethod]
        public void InjectCodehandler_NoFreeSpace_Throws()
        {
            var dol = new byte[0x400];
            for (int i = 0; i < dol.Length; i++) dol[i] = 0xFF;
            var codehandler = new byte[] { 1, 2, 3, 4 };

            Assert.ThrowsException<InvalidOperationException>(() => sut.InjectCodehandler(dol, codehandler));
        }

        /// <summary>
        /// InjectCodehandler throws ArgumentException when codehandler is empty.
        /// </summary>
        [TestMethod]
        public void InjectCodehandler_EmptyCodehandler_Throws()
        {
            var dol = new byte[0x400];
            var codehandler = new byte[0];

            Assert.ThrowsException<ArgumentException>(() => sut.InjectCodehandler(dol, codehandler));
        }

        private static byte[] BuildMinimalDol(uint fileOffset, uint memoryAddress, uint size)
        {
            var dol = new byte[0x300];
            WriteBE(dol, 0x00, fileOffset);
            WriteBE(dol, 0x48, memoryAddress);
            WriteBE(dol, 0x90, size);
            return dol;
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
