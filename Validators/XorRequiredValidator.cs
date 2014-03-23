using System.Linq;

namespace BusinessObjects.Validators
{
    public class XorRequiredValidator : Validator {

        public XorRequiredValidator(string[] properties) : base(properties, "Required."){ }
        public XorRequiredValidator(string[] properties, string description) : base(properties, description){ }

        /// <summary>
        /// Validates that the rule has been followed.
        /// </summary>
        public override bool Validate(BusinessObject businessObject) {
            var cnt = Properties.Select(prop => GetPropertyValue(businessObject, prop)).Count(v => v != null);
            return cnt == 1;
        }
    }
}
