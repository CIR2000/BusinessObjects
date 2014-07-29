using BusinessObjects;

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
            if (v is string)
                return !string.IsNullOrEmpty((string)v);
            if (v is BusinessObjectBase) {
                var o = (BusinessObjectBase)v;
                return !o.IsEmpty();
            }
            return v != null;
        }
    }
}
