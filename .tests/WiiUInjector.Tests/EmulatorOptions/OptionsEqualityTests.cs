#nullable disable
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WiiUInjector.Domain;
using WiiUInjector.Domain.EmulatorOptions;

namespace WiiUInjector.Tests.EmulatorOptions
{
    [TestClass]
    public class OptionsEqualityTests
    {
        /// <summary>
        /// N64Options with same fields are equal.
        /// </summary>
        [TestMethod]
        public void N64Options_SameFields_Equal()
        {
            var opt1 = new N64Options(true, false);
            var opt2 = new N64Options(true, false);

            Assert.IsTrue(opt1.Equals(opt2));
            Assert.IsTrue(opt1 == opt2);
            Assert.IsFalse(opt1 != opt2);
            Assert.AreEqual(opt1.GetHashCode(), opt2.GetHashCode());
        }

        /// <summary>
        /// N64Options with different fields are not equal.
        /// </summary>
        [TestMethod]
        public void N64Options_DifferentFields_NotEqual()
        {
            var opt1 = new N64Options(true, false);
            var opt2 = new N64Options(false, false);

            Assert.IsFalse(opt1.Equals(opt2));
            Assert.IsTrue(opt1 != opt2);
            Assert.IsFalse(opt1 == opt2);
        }

        /// <summary>
        /// WiiOptions with same fields are equal.
        /// </summary>
        [TestMethod]
        public void WiiOptions_SameFields_Equal()
        {
            var opt1 = new WiiOptions(true, false, true, false, false, false, false, false, true, ControllerLayout.Wiimote, false);
            var opt2 = new WiiOptions(true, false, true, false, false, false, false, false, true, ControllerLayout.Wiimote, false);

            Assert.IsTrue(opt1.Equals(opt2));
            Assert.IsTrue(opt1 == opt2);
            Assert.IsFalse(opt1 != opt2);
            Assert.AreEqual(opt1.GetHashCode(), opt2.GetHashCode());
        }

        /// <summary>
        /// WiiOptions with different ControllerLayout are not equal.
        /// </summary>
        [TestMethod]
        public void WiiOptions_DifferentControllerLayout_NotEqual()
        {
            var opt1 = new WiiOptions(false, false, false, false, false, false, false, false, false, ControllerLayout.GamePad, false);
            var opt2 = new WiiOptions(false, false, false, false, false, false, false, false, false, ControllerLayout.Wiimote, false);

            Assert.IsFalse(opt1.Equals(opt2));
            Assert.IsTrue(opt1 != opt2);
            Assert.IsFalse(opt1 == opt2);
        }

        /// <summary>
        /// GcnOptions with same fields are equal.
        /// </summary>
        [TestMethod]
        public void GcnOptions_SameFields_Equal()
        {
            var opt1 = new GcnOptions(true, false, true);
            var opt2 = new GcnOptions(true, false, true);

            Assert.IsTrue(opt1.Equals(opt2));
            Assert.IsTrue(opt1 == opt2);
            Assert.IsFalse(opt1 != opt2);
            Assert.AreEqual(opt1.GetHashCode(), opt2.GetHashCode());
        }

        /// <summary>
        /// GcnOptions with different fields are not equal.
        /// </summary>
        [TestMethod]
        public void GcnOptions_DifferentFields_NotEqual()
        {
            var opt1 = new GcnOptions(true, false, true);
            var opt2 = new GcnOptions(false, false, true);

            Assert.IsFalse(opt1.Equals(opt2));
            Assert.IsTrue(opt1 != opt2);
            Assert.IsFalse(opt1 == opt2);
        }

        /// <summary>
        /// GbaOptions with same fields are equal.
        /// </summary>
        [TestMethod]
        public void GbaOptions_SameFields_Equal()
        {
            var opt1 = new GbaOptions(true, false);
            var opt2 = new GbaOptions(true, false);

            Assert.IsTrue(opt1.Equals(opt2));
            Assert.IsTrue(opt1 == opt2);
            Assert.IsFalse(opt1 != opt2);
            Assert.AreEqual(opt1.GetHashCode(), opt2.GetHashCode());
        }

        /// <summary>
        /// NesSnesOptions with same console and flag are equal.
        /// </summary>
        [TestMethod]
        public void NesSnesOptions_SameConsoleAndFlag_Equal()
        {
            var opt1 = new NesSnesOptions(ConsoleType.Nes, true);
            var opt2 = new NesSnesOptions(ConsoleType.Nes, true);

            Assert.IsTrue(opt1.Equals(opt2));
            Assert.IsTrue(opt1 == opt2);
            Assert.IsFalse(opt1 != opt2);
            Assert.AreEqual(opt1.GetHashCode(), opt2.GetHashCode());
        }

        /// <summary>
        /// NesSnesOptions with different consoles are not equal.
        /// </summary>
        [TestMethod]
        public void NesSnesOptions_DifferentConsole_NotEqual()
        {
            var opt1 = new NesSnesOptions(ConsoleType.Nes, true);
            var opt2 = new NesSnesOptions(ConsoleType.Snes, true);

            Assert.IsFalse(opt1.Equals(opt2));
            Assert.IsTrue(opt1 != opt2);
            Assert.IsFalse(opt1 == opt2);
        }

        /// <summary>
        /// NesSnesOptions throws ArgumentException for invalid console.
        /// </summary>
        [TestMethod]
        public void NesSnesOptions_InvalidConsole_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => new NesSnesOptions(ConsoleType.Wii, true));
        }

        /// <summary>
        /// TurboGrafxOptions with same fields are equal.
        /// </summary>
        [TestMethod]
        public void TurboGrafxOptions_SameFields_Equal()
        {
            var opt1 = new TurboGrafxOptions(true);
            var opt2 = new TurboGrafxOptions(true);

            Assert.IsTrue(opt1.Equals(opt2));
            Assert.IsTrue(opt1 == opt2);
            Assert.IsFalse(opt1 != opt2);
            Assert.AreEqual(opt1.GetHashCode(), opt2.GetHashCode());
        }

        /// <summary>
        /// DsOptions with same fields are equal.
        /// </summary>
        [TestMethod]
        public void DsOptions_SameFields_Equal()
        {
            var opt1 = new DsOptions(DsLayout.Horizontal, StLayout.Both, 2, 75, PixelArtUpscaler.Hq2x);
            var opt2 = new DsOptions(DsLayout.Horizontal, StLayout.Both, 2, 75, PixelArtUpscaler.Hq2x);

            Assert.IsTrue(opt1.Equals(opt2));
            Assert.IsTrue(opt1 == opt2);
            Assert.IsFalse(opt1 != opt2);
            Assert.AreEqual(opt1.GetHashCode(), opt2.GetHashCode());
        }

        /// <summary>
        /// DsOptions throws ArgumentOutOfRangeException for negative brightness.
        /// </summary>
        [TestMethod]
        public void DsOptions_InvalidBrightness_Throws()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                new DsOptions(DsLayout.Horizontal, StLayout.Both, 1, -1, PixelArtUpscaler.None));
        }

        /// <summary>
        /// DsOptions throws ArgumentOutOfRangeException for brightness over 100.
        /// </summary>
        [TestMethod]
        public void DsOptions_BrightnessOver100_Throws()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                new DsOptions(DsLayout.Horizontal, StLayout.Both, 1, 101, PixelArtUpscaler.None));
        }

        /// <summary>
        /// DsOptions throws ArgumentOutOfRangeException for invalid renderer scale.
        /// </summary>
        [TestMethod]
        public void DsOptions_InvalidRendererScale_Throws()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                new DsOptions(DsLayout.Horizontal, StLayout.Both, 0, 50, PixelArtUpscaler.None));
        }

        /// <summary>
        /// NoOptions with same console are equal.
        /// </summary>
        [TestMethod]
        public void NoOptions_SameConsole_Equal()
        {
            var opt1 = new NoOptions(ConsoleType.Msx);
            var opt2 = new NoOptions(ConsoleType.Msx);

            Assert.IsTrue(opt1.Equals(opt2));
            Assert.IsTrue(opt1 == opt2);
            Assert.IsFalse(opt1 != opt2);
            Assert.AreEqual(opt1.GetHashCode(), opt2.GetHashCode());
        }

        /// <summary>
        /// NoOptions with different consoles are not equal.
        /// </summary>
        [TestMethod]
        public void NoOptions_DifferentConsole_NotEqual()
        {
            var opt1 = new NoOptions(ConsoleType.Msx);
            var opt2 = new NoOptions(ConsoleType.Nes);

            Assert.IsFalse(opt1.Equals(opt2));
            Assert.IsTrue(opt1 != opt2);
            Assert.IsFalse(opt1 == opt2);
        }

        /// <summary>
        /// EmulatorOptionsDefaults.For returns options with matching console.
        /// </summary>
        [TestMethod]
        [DataRow(ConsoleType.Nds)]
        [DataRow(ConsoleType.N64)]
        [DataRow(ConsoleType.Gba)]
        [DataRow(ConsoleType.Nes)]
        [DataRow(ConsoleType.Snes)]
        [DataRow(ConsoleType.TurboGrafx)]
        [DataRow(ConsoleType.Msx)]
        [DataRow(ConsoleType.Wii)]
        [DataRow(ConsoleType.Gcn)]
        public void EmulatorOptionsDefaults_AllConsoleTypes_ReturnsMatchingOptions(ConsoleType console)
        {
            var options = EmulatorOptionsDefaults.For(console);
            Assert.AreEqual(console, options.Console);
        }
    }
}
