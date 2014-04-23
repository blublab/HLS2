using Common.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Util.Common.DataTypes;

namespace Tests.KomponentenTest.Common
{
    [TestClass]
    public class KomponentenTest_Common_DataTypes
    {
        [TestMethod]
        public void TestEMailTypSuccess()
        {
            Assert.IsTrue(EMailType.IsValid("stefan.sarstedt@haw-hamburg.de"));
        }

        [TestMethod]
        public void TestEMailTypKonstruktorSuccess()
        {
            EMailType emailType = new EMailType("stefan.sarstedt@haw-hamburg.de");
        }

        [TestMethod]
        public void TestEMailTypFail()
        {
            Assert.IsFalse(EMailType.IsValid("stefan.sarstedthaw-hamburg.de"));
            Assert.IsFalse(EMailType.IsValid("stefan.sarstedt@haw-hamburg."));
            Assert.IsFalse(EMailType.IsValid("stefan.sarstedt@haw-hamburg"));
            Assert.IsFalse(EMailType.IsValid("@haw-hamburg.de"));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestEMailTypKonstruktorFail()
        {
            EMailType emailType = new EMailType("@haw-hamburg.de");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestKontodatenTypFail()
        {
            KontodatenType kdT = new KontodatenType("asdodsaji", "foiewof");
        }

        [TestMethod]
        public void TestKontodatenTypSuccess()
        {
            KontodatenType kdT = new KontodatenType("DE00210501700012345678", "RZTIAT22263");
        }

        [TestMethod]
        public void TestWaehrungsTypAddition()
        {
            WaehrungsType w1 = new WaehrungsType(3);
            WaehrungsType w2 = new WaehrungsType(4);
            WaehrungsType w3 = w1+w2;
            decimal erg = 7;
            Assert.IsTrue(w3.Wert == erg);
        }

        [TestMethod]
        public void TestWaehrungsTypSubtrktion()
        {
            WaehrungsType w1 = new WaehrungsType(10);
            WaehrungsType w2 = new WaehrungsType(8);
            WaehrungsType w3 = w1 - w2;
            decimal erg = 2;
            Assert.IsTrue(w3.Wert == erg);
        }

        [TestMethod]
        public void TestWaehrungsTypMultiplikation()
        {
            WaehrungsType w1 = new WaehrungsType(3);
            WaehrungsType w2 = new WaehrungsType(4);
            WaehrungsType w3 = w1 * w2;
            decimal erg = 12;
            Assert.IsTrue(w3.Wert == erg);
        }

        [TestMethod]
        public void TestWaehrungsTypAdditionDivision()
        {
            WaehrungsType w1 = new WaehrungsType(20);
            WaehrungsType w2 = new WaehrungsType(4);
            WaehrungsType w3 = w1 / w2;
            decimal erg = 5;
            Assert.IsTrue(w3.Wert == erg);
        }
    }
}
