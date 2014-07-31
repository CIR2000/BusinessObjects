using BusinessObjects.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Xml;

namespace BusinessObjects.Tests
{
    [TestClass]
    public class BaseClass
    {
        protected static ComplexObject GetMock()
        {
            var o = new ComplexObject {RequiredProperty = "hello"};
            o.SimpleObject.AnotherProperty = "AnotherProperty";
            o.SimpleObject.SimpleProperty = "SimpleProperty";
            return o;
        }

        protected static void AssertValidObject(BusinessObjectBase o)
        {
            Assert.IsTrue(o.IsValid);
            Assert.IsNull(o.Error);
            Assert.AreEqual(o.GetBrokenRules().Count, 0);
        }

        protected static void AssertValidObject(BusinessObjectBase o, string property)
        {
            AssertValidObject(o);
            AssertValidProperty(o, property);
        }

        protected static void AssertValidProperty(BusinessObjectBase o, string property)
        {
            Assert.AreEqual(o.GetBrokenRules(property).Count, 0);
            Assert.IsNull(o[property]);
        }

        protected static void AssertInvalidObject(BusinessObjectBase o)
        {
            Assert.IsFalse(o.IsValid);
            Assert.AreEqual(o.GetBrokenRules().Count, 1);
        }

        protected static void AssertInvalidObject(BusinessObjectBase o, string property, Type expectedValidatorType)
        {
            AssertInvalidObject(o);
            AssertInvalidProperty(o, property, expectedValidatorType);
        }

        protected static void AssertInvalidProperty(BusinessObjectBase o, string property, Type expectedValidatorType)
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
