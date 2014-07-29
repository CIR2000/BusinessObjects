using System;
using System.IO;
using System.Xml;
using BusinessObjects;
using BusinessObjects.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinessObjects.Tests
{
    [TestClass]
    public class BusinessObjectTests
    {


        [TestMethod]
        public void IsEmpty()
        {
            var o = new SimpleObject();
            Assert.IsTrue(o.IsEmpty());

            o.SimpleProperty = "hello";
            Assert.IsFalse(o.IsEmpty());
        }

        [TestMethod]
        public void NonExistingProperty()
        {
            var o = GetMock();
            Assert.AreEqual(o.GetBrokenRules("non_existing_property").Count, 0);
            Assert.IsNull(o["non_existing_property"]);
        }

        [TestMethod]
        public void RequiredValidator()
        {

            const string propertyName = "RequiredProperty";

            // instantiate an invalid object (required property is null).
            var o = new ComplexObject();
            AssertInvalidObject(o, propertyName, typeof(Validator));

            o.RequiredProperty = "hello";
            AssertValidObject(o, propertyName);
        }

        [TestMethod]
        public void LengthValidator()
        {

            const string propertyName = "LengthProperty";
            var expectedValidatorType = typeof (LengthValidator);

            var o = GetMock();
            o.LengthProperty = "too long for ya";
            AssertInvalidObject(o, propertyName, expectedValidatorType);

            o.LengthProperty = "hello";
            AssertValidObject(o, propertyName);
        }

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

        private static ComplexObject GetMock()
        {
            var o = new ComplexObject {RequiredProperty = "hello"};
            o.SimpleObject.AnotherProperty = "AnotherProperty";
            o.SimpleObject.SimpleProperty = "SimpleProperty";
            return o;
        }

        private static void AssertValidObject(BusinessObjectBase o)
        {
            Assert.IsTrue(o.IsValid);
            Assert.IsNull(o.Error);
            Assert.AreEqual(o.GetBrokenRules().Count, 0);
        }

        private static void AssertValidObject(BusinessObjectBase o, string property)
        {
            AssertValidObject(o);
            AssertValidProperty(o, property);
        }

        private static void AssertValidProperty(BusinessObjectBase o, string property)
        {
            Assert.AreEqual(o.GetBrokenRules(property).Count, 0);
            Assert.IsNull(o[property]);
        }

        private static void AssertInvalidObject(BusinessObjectBase o)
        {
            Assert.IsFalse(o.IsValid);
            Assert.AreEqual(o.GetBrokenRules().Count, 1);
        }

        private static void AssertInvalidObject(BusinessObjectBase o, string property, Type expectedValidatorType)
        {
            AssertInvalidObject(o);
            AssertInvalidProperty(o, property, expectedValidatorType);
        }

        private static void AssertInvalidProperty(BusinessObjectBase o, string property, Type expectedValidatorType)
        {
            Assert.AreEqual(o.GetBrokenRules(property).Count, 1);
            Assert.IsNotNull(o[property]);

            var v = o.GetBrokenRules()[0];
            Assert.AreEqual(property, v.PropertyName);
            Assert.IsNotNull(v.Description);
            Assert.IsInstanceOfType(v, expectedValidatorType);
        }
    }
}
