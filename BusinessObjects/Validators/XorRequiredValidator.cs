using System.Linq;
using BusinessObjects;

namespace BusinessObjects.Validators
{
    /// <summary>
    /// Validates that at least one property value has been set (not null).
    /// </summary>
    public class XorRequiredValidator : Validator {

        /// <summary>
        /// Validates that at least one property value has been set (not null).
        /// </summary>
        /// <param name="properties"></param>
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
