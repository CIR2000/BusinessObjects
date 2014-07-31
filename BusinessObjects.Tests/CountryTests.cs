using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinessObjects.Tests
{
    [TestClass]
    public class CountryTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            const int expectedLength = 250;
            Assert.AreEqual(Country.List.Length, expectedLength);
            Assert.AreEqual(Country.TwoLetterCodes.Length, expectedLength);
            Assert.AreEqual(Country.ThreeLetterCodes.Length, expectedLength);
            Assert.AreEqual(Country.NumericCodes.Length, expectedLength);
            Assert.AreEqual(Country.Names.Length, expectedLength);

            Assert.AreEqual(Country.List[1].TwoLetterCode, "AF");
            Assert.AreEqual(Country.TwoLetterCodes[1], "AF");
            Assert.AreEqual(Country.ThreeLetterCodes[1], "AFG");
            Assert.AreEqual(Country.NumericCodes[1], "004");
            Assert.AreEqual(Country.Names[1], "Afghanistan");

        }
    }
}
