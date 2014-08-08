using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using BusinessObjects.Validators;

namespace BusinessObjects.Tests
{
    [TestClass]
    public class ValidatorTests
    {
        const string PropertyName = "SimpleProperty";
        const string Description = "Houston we have a problem.";

        [TestMethod]
        public void LengthValidator()
        {
            var o = new SimpleObject();
            var v = new LengthValidator(PropertyName, Description, 2, 5);

            AssertValidatorProperties(v);

            Assert.IsTrue(v.Validate(o));

            o.SimpleProperty = string.Empty;
            Assert.IsTrue(v.Validate(o));
            o.SimpleProperty = "12";
            Assert.IsTrue(v.Validate(o));
            o.SimpleProperty = "123";
            Assert.IsTrue(v.Validate(o));
            o.SimpleProperty = "12345";
            Assert.IsTrue(v.Validate(o));

            o.SimpleProperty = "1";
            Assert.IsFalse(v.Validate(o));
            o.SimpleProperty = "123456";
            Assert.IsFalse(v.Validate(o));

        }

        [TestMethod]
        public void RequiredValidator()
        {
            var v = new RequiredValidator(PropertyName, Description);
            AssertValidatorProperties(v);

            var o = new SimpleObject {SimpleProperty = "hello"};
            Assert.IsTrue(v.Validate(o));

            o.SimpleProperty = string.Empty;
            Assert.IsFalse(v.Validate(o));
            Assert.IsFalse(v.Validate(o));
        }

        [TestMethod]
        public void DomainValidator()
        {
            var v = new DomainValidator(PropertyName, Description, new []{"ONE", "TWO"});
            AssertValidatorProperties(v);

            var o = new SimpleObject();
            Assert.IsTrue(v.Validate(o));
            o.SimpleProperty = string.Empty;
            Assert.IsTrue(v.Validate(o));
            o.SimpleProperty = "ONE";
            Assert.IsTrue(v.Validate(o));
            o.SimpleProperty = "TWO";
            Assert.IsTrue(v.Validate(o));

            o.SimpleProperty = "THREE";
            Assert.IsFalse(v.Validate(o));
        }

        [TestMethod]
        public void CountryValidator()
        {
            var v = new CountryValidator(PropertyName, Description);
            AssertValidatorProperties(v);

            var o = new SimpleObject();
            Assert.IsTrue(v.Validate(o));
            o.SimpleProperty = string.Empty;
            Assert.IsTrue(v.Validate(o));
            o.SimpleProperty = "IT";
            Assert.IsTrue(v.Validate(o));

            o.SimpleProperty = "XX";
            Assert.IsFalse(v.Validate(o));
        }

        [TestMethod]
        public void DelegateValidator()
        {
            var o = new SimpleObject();
            var v = new DelegateValidator(PropertyName, Description, () => o.SimpleProperty == "hello");
            AssertValidatorProperties(v);

            o.SimpleProperty = "hello";
            Assert.IsTrue(v.Validate(o));

            o.SimpleProperty = "bye";
            Assert.IsFalse(v.Validate(o));
        }

        [TestMethod]
        public void RegexValidator()
        {
            var v = new RegexValidator(PropertyName, Description, @"^(0|[1-9][0-9]*)$");
            AssertValidatorProperties(v);

            var o = new SimpleObject {SimpleProperty = "1234567890"};
            Assert.IsTrue(v.Validate(o));

            o.SimpleProperty = "ABC1234567890";
            Assert.IsFalse(v.Validate(o));
        }

        [TestMethod]
        public void AndCompositeValidator()
        {
            var v = new AndCompositeValidator(PropertyName, new List<Validator>{ new RequiredValidator(), new LengthValidator(5)});
            Assert.AreEqual(v.PropertyName, PropertyName);

            var o = new SimpleObject {SimpleProperty = "12345"};
            Assert.IsTrue(v.Validate(o));

            o.SimpleProperty = null;
            Assert.IsFalse(v.Validate(o));
            o.SimpleProperty = "123";
            Assert.IsFalse(v.Validate(o));
            o.SimpleProperty = "123456";
            Assert.IsFalse(v.Validate(o));
        }
        [TestMethod]
        public void XorRequiredValidator(){
        
            var v = new XorRequiredValidator(new []{PropertyName, "AnotherProperty"});

            // ReSharper disable once UseObjectOrCollectionInitializer
            var o = new SimpleObject();
            o.SimpleProperty = null;
            o.AnotherProperty = "12345";
            Assert.IsTrue(v.Validate(o));
            o.SimpleProperty = string.Empty;
            Assert.IsTrue(v.Validate(o));
            o.SimpleProperty = "12345";
            o.AnotherProperty = null;
            Assert.IsTrue(v.Validate(o));
            o.AnotherProperty = string.Empty;
            Assert.IsTrue(v.Validate(o));

            o.SimpleProperty = null;
            o.AnotherProperty = null;
            Assert.IsFalse(v.Validate(o));
            o.SimpleProperty = string.Empty;
            o.AnotherProperty = string.Empty;
            Assert.IsFalse(v.Validate(o));

            // Since its an exclusive or we also won't validate if more than one property has values.
            o.SimpleProperty = "12345";
            o.AnotherProperty = "67890";
            Assert.IsFalse(v.Validate(o));
        }
        public void AssertValidatorProperties(Validator v)
        {
            Assert.AreEqual(v.PropertyName, PropertyName);
            Assert.AreEqual(v.Description, Description);
        }
    }
}
