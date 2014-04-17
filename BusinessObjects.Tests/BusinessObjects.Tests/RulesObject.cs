using BusinessObjects.Validators;

namespace BusinessObjects.Tests
{
    public class RulesObject : BusinessObject {
        public override string XmlName { get { return "RulesObject"; } }

        protected override System.Collections.Generic.List<Validators.Validator> CreateRules()
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
    }
}