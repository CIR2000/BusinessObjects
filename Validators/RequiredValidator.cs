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
        public override bool Validate(BusinessObject businessObject) {
            var v = GetPropertyValue(businessObject);
            if (v is string)
                return !string.IsNullOrEmpty((string)v);
            else if (v is BusinessObject) {
                var o = (BusinessObject)v;
                return !o.IsEmpty();
            }
            else
                return v != null;
        }
    }
}
