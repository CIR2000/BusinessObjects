namespace BusinessObjects.Validators {
    public class RegexValidator : Validator {

        /// <summary>
        /// Constructor.
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
            var v = (string)GetPropertyValue(businessObject);
            return string.IsNullOrEmpty(v) || System.Text.RegularExpressions.Regex.Match(v, Regex).Success;
        }
    }
}
