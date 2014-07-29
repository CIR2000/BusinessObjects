using System.Xml;
using BusinessObjects.Validators;
using BusinessObjects;

namespace BusinessObjects.Tests
{
    public class ComplexObject : BusinessObject
    {
        private readonly SimpleObject _simple;

        public ComplexObject()
        {
            _simple = new SimpleObject();
        }
        public ComplexObject(XmlReader r) { ReadXml(r); }

        protected override System.Collections.Generic.List<Validator> CreateRules()
        {
            var rules = base.CreateRules();
            rules.Add(new LengthValidator("LengthProperty", 1, 5));
            rules.Add(new RequiredValidator("RequiredProperty"));
            return rules;
        }
        [OrderedDataProperty]
        public string LengthProperty { get; set; }

        [OrderedDataProperty]
        public string RequiredProperty { get; set; }

        [OrderedDataProperty]
        public SimpleObject SimpleObject { get { return _simple; } }
    }
}