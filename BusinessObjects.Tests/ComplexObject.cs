using BusinessObjects.Validators;
using System.Xml;

namespace BusinessObjects.Tests
{
    public class ComplexObject : BusinessObject
    {
        private readonly SimpleObject _simple;

        public ComplexObject()
        {
            _simple = new SimpleObject();
            DelegateProperty = "dummy";
            FirstProperty = "first";
            AndProperty = "And";
        }
        public ComplexObject(XmlReader r) { ReadXml(r); }

        protected override System.Collections.Generic.List<Validator> CreateRules()
        {
            var rules = base.CreateRules();
            rules.Add(new LengthValidator("LengthProperty", 1, 5));
            rules.Add(new RequiredValidator("RequiredProperty"));
            rules.Add(new CountryValidator("CountryProperty"));
            rules.Add(new DelegateValidator("DelegateProperty", "This is a fail", () => (DelegateProperty == "dummy")));
            rules.Add(new RegexValidator("RegexProperty", "dummy"));
            rules.Add(new XorRequiredValidator(new[] {"FirstProperty", "SecondProperty"}, "This is a fail"));
            rules.Add(
                new AndCompositeValidator(
                    "AndProperty",
                    new System.Collections.Generic.List<Validator> {
                        new LengthValidator(3),
                        new DelegateValidator(null, "fail", () => (AndProperty == "And"))
                    }));
            return rules;
        }
        [DataProperty]
        public string LengthProperty { get; set; }

        [DataProperty]
        public string RequiredProperty { get; set; }

        [DataProperty (order:3)]
        public string CountryProperty { get; set; }

        [DataProperty (order:4)]
        public string DelegateProperty { get; set; }

        [DataProperty]
        public string RegexProperty { get; set; }

        [DataProperty (order:0)]
        public string FirstProperty { get; set; }

        [DataProperty (order:1)]
        public string SecondProperty { get; set; }

        [DataProperty (order:2)]
        public string AndProperty { get; set; }

        [DataProperty (order:5)]
        public SimpleObject SimpleObject { get { return _simple; } }
    }
}