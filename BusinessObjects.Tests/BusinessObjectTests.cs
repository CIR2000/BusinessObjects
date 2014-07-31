using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;

namespace BusinessObjects.Tests
{
    [TestClass]
    public class BusinessObjectTests : BaseClass
    {
        [TestMethod]
        public void SerializeXml()
        {
            var o = GetMock();
            var f = Path.GetTempFileName();
            var settings = new XmlWriterSettings {Indent = true};

            using (var writer = XmlWriter.Create(f, settings))
            {
                writer.WriteStartElement("root");
                o.WriteXml(writer);
                writer.WriteEndElement();
            }

            var o1 = new ComplexObject();
            var s = new XmlReaderSettings {IgnoreWhitespace = true};
            var r = XmlReader.Create(f, s);
            o1.ReadXml(r);
            Assert.IsTrue(o.Equals(o1));
        }
    }
}
