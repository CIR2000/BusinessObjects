using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BusinessObjects.PCL;

namespace BusinessObjects.Tests
{
    [TestClass]
    public class CountryTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            const int expectedLength = 249;
            Assert.AreEqual(Country.List.Length, expectedLength);
            Assert.AreEqual(Country.TwoLetterCodes.Length, expectedLength);
            Assert.AreEqual(Country.ThreeLetterCodes.Length, expectedLength);
            Assert.AreEqual(Country.NumericCodes.Length, expectedLength);
            Assert.AreEqual(Country.Names.Length, expectedLength);

            Assert.AreEqual(Country.List[0].TwoLetterCode, "AF");
            Assert.AreEqual(Country.TwoLetterCodes[0], "AF");
            Assert.AreEqual(Country.ThreeLetterCodes[0], "AFG");
            Assert.AreEqual(Country.NumericCodes[0], "004");
            Assert.AreEqual(Country.Names[0], "Afghanistan");

        }
    }
}
