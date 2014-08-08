using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml;

namespace BusinessObjects.Tests
{
    [TestClass]
    public class BusinessObjectTests : BaseClass
    {
        [TestMethod]
        public void XmlSerializedObjectEqualsOriginalObject()
        {
            var original = GetMock();
            var fileName = WriteXml(original);

            // Test that retrieved object equals original object
            var serialized = new ComplexObject();
            var s = new XmlReaderSettings {IgnoreWhitespace = true};
            var r = XmlReader.Create(fileName, s);
            serialized.ReadXml(r);
            Assert.IsTrue(serialized.Equals(original));
        }

        [TestMethod]
        public void XmlSerializedElementsMatchPropertiesOrder()
        {
            var o = GetMock();
            o.SecondProperty = "second";

            var fileName = WriteXml(o);

            //var o1 = new ComplexObject();
            var s = new XmlReaderSettings {IgnoreWhitespace = true};
            using (var r = XmlReader.Create(fileName, s))
            {
                r.MoveToContent();
                r.ReadStartElement("root");
                
                // test that properties have been serialized in the proper order
                Assert.IsTrue(r.ReadElementContentAsString("FirstProperty", string.Empty) == "first");
                Assert.IsTrue(r.ReadElementContentAsString("SecondProperty", string.Empty) == "second");
                Assert.IsTrue(r.ReadElementContentAsString("AndProperty", string.Empty) == "And");
                // CountryProperty is 4th but it wasn't serialized as empty value, so we proceed on next one: DelegateProperty
                Assert.IsTrue(r.ReadElementContentAsString("DelegateProperty", string.Empty) == "dummy");

                // SimpleObject is next. Test that child BusinessObjects are properly serialized
                r.ReadStartElement("SimpleObject");
                Assert.IsTrue(r.ReadElementContentAsString("AnotherProperty", string.Empty) == "AnotherProperty");
                Assert.IsTrue(r.ReadElementContentAsString("SimpleProperty", string.Empty) == "SimpleProperty");
                r.ReadEndElement();

                // RequiredProperty has no explictly set order, but it will be next as it is the only non-empty property left in the object
                Assert.IsTrue(r.ReadElementContentAsString("RequiredProperty", string.Empty) == "hello");
            }
            
        }

        [TestMethod]
        public void XmlSerializeNullValues()
        {
            var o = GetMock();
            o.FirstProperty = null;
            o.XmlOptions.SerializeNullValues = true;
            

            var fileName = WriteXml(o);

            var s = new XmlReaderSettings {IgnoreWhitespace = true};
            using (var r = XmlReader.Create(fileName, s))
            {
                r.MoveToContent();
                r.ReadStartElement("root");

                Assert.IsTrue(r.IsEmptyElement);
                Assert.IsTrue(r.ReadElementContentAsString("FirstProperty", string.Empty) == "");
            }
        }


        [TestMethod]
        public void XmlSerializeEmptyStrings()
        {
            var o = GetMock();
            o.FirstProperty = string.Empty;
            o.XmlOptions.SerializeEmptyStrings = true;
            

            var fileName = WriteXml(o);

            var s = new XmlReaderSettings {IgnoreWhitespace = true};
            using (var r = XmlReader.Create(fileName, s))
            {
                r.MoveToContent();
                r.ReadStartElement("root");
                Assert.IsTrue(r.IsEmptyElement);
                Assert.IsTrue(r.ReadElementContentAsString("FirstProperty", string.Empty) == "");

                // SecondProperty was null but it was serialized anyway since SerializeEmtpyOrNullStrings is true
            }
        }
        private static
            XmlWriter GetXmlWriter(string fileName)
        {
            var settings = new XmlWriterSettings();
            return XmlWriter.Create(fileName, settings);
        }

        private static string WriteXml(IXmlSerializable obj)
        {
            var f = Path.GetTempFileName();
            //var f = "test.xml";
            using (var writer = GetXmlWriter(f))
            {
                writer.WriteStartElement("root");
                obj.WriteXml(writer);
                writer.WriteEndElement();
            }
            return f;
        }
    }
}
