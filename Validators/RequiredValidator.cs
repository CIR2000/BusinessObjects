namespace BusinessObjects.Validators {
    public class RequiredValidator : Validator {

        /// <summary>
        /// Constructor.
        /// </summary>
        public RequiredValidator() : base(null, "Required.") { }
        public RequiredValidator(string propertyName) : base(propertyName, "Required.") { }
        public RequiredValidator(string propertyName, string description) : base(propertyName, description) { }

        /// <summary>
        /// Validates that the rule has been followed.
        /// </summary>
        public override bool Validate(DomainObject domainObject) {
            var v = GetPropertyValue(domainObject);
            if (v is string)
                return !string.IsNullOrEmpty((string)v);
            else if (v is DomainObject) {
                var o = (DomainObject)v;
                return !o.IsEmpty();
            }
            else
                return v != null;
        }
    }
}
