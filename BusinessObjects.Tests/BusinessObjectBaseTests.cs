using BusinessObjects.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Xml;

namespace BusinessObjects.Tests
{
    [TestClass]
    public class BusinessObjectBaseTests : BaseClass
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
    }
}
