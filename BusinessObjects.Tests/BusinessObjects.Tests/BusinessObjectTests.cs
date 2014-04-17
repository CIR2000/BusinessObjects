using System;
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
            var o = new RulesObject();
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

        private static RulesObject GetMock()
        {
            return new RulesObject {RequiredProperty = "hello"};
        }

        private static void AssertValidObject(BusinessObject o)
        {
            Assert.IsTrue(o.IsValid);
            Assert.IsNull(o.Error);
            Assert.AreEqual(o.GetBrokenRules().Count, 0);
        }

        private static void AssertValidObject(BusinessObject o, string property)
        {
            AssertValidObject(o);
            AssertValidProperty(o, property);
        }

        private static void AssertValidProperty(BusinessObject o, string property)
        {
            Assert.AreEqual(o.GetBrokenRules(property).Count, 0);
            Assert.IsNull(o[property]);
        }

        private static void AssertInvalidObject(BusinessObject o)
        {
            Assert.IsFalse(o.IsValid);
            Assert.AreEqual(o.GetBrokenRules().Count, 1);
        }

        private static void AssertInvalidObject(BusinessObject o, string property, Type expectedValidatorType)
        {
            AssertInvalidObject(o);
            AssertInvalidProperty(o, property, expectedValidatorType);
        }

        private static void AssertInvalidProperty(BusinessObject o, string property, Type expectedValidatorType)
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
