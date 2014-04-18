using System.Xml;
using BusinessObjects.Validators;

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
        public ComplexObject(string fileName) { ReadXml(fileName); }

        public override string XmlName { get { return "ComplexObject"; } }

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