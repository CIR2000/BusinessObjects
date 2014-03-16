using System.Text.RegularExpressions;

namespace BusinessObjects.Validators {
    public class RegexValidator : Validator {

        private string _regex;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RegexValidator(string propertyName, string regex) : this(propertyName, "Unrecognized format.", regex) { }
        public RegexValidator(string propertyName, string description, string regex) : base(propertyName, description) {
            _regex = regex;
        }

        /// <summary>
        /// Validates that the rule has been followed.
        /// </summary>
        public override bool Validate(DomainObject domainObject) {
            string v = (string)GetPropertyValue(domainObject);
            return string.IsNullOrEmpty(v) || Regex.Match(v, _regex).Success;
        }
    }
}
