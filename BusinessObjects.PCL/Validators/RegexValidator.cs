using BusinessObjects;

namespace BusinessObjects.Validators {
    /// <summary>
    /// Validates that a property value matches a regex.
    /// </summary>
    public class RegexValidator : Validator {

        /// <summary>
        /// Validates that a property value matches a regex.
        /// </summary>
        public RegexValidator(string propertyName, string regex) : this(propertyName, "Unrecognized format.", regex) { }
        public RegexValidator(string propertyName, string description, string regex) : base(propertyName, description) {
            Regex = regex;
        }

        public string Regex { get; set; }

        /// <summary>
        /// Validates that the rule has been followed.
        /// </summary>
        public override bool Validate(BusinessObject businessObject) {
            var v = (string)GetPropertyValue(businessObject, PropertyName);
            return string.IsNullOrEmpty(v) || System.Text.RegularExpressions.Regex.Match(v, Regex).Success;
        }
    }
}
