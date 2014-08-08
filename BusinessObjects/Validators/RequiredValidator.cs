namespace BusinessObjects.Validators {
    /// <summary>
    /// Validates that a property value is not null or, if of BusinessObject type, not empty.
    /// </summary>
    public class RequiredValidator : Validator {

        /// <summary>
        /// Validates that a property value is not null or, if of BusinessObject type, not empty.
        /// </summary>
        public RequiredValidator() : base("Required.") { }
        public RequiredValidator(string propertyName) : base(propertyName, "Required.") { }
        public RequiredValidator(string propertyName, string description) : base(propertyName, description) { }

        /// <summary>
        /// Validates that the rule has been followed.
        /// </summary>
        public override bool Validate(BusinessObjectBase businessObject) {
            var v = GetPropertyValue(businessObject, PropertyName);

            var @string = v as string;
            if (@string != null) {
                return !string.IsNullOrEmpty((string)v);
            }

            var @base = v as BusinessObjectBase;
            if (@base != null) {
                return !@base.IsEmpty();
            }

            return v != null;
        }
    }
}
