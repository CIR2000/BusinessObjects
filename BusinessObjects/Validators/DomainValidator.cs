using System;
using BusinessObjects;

namespace BusinessObjects.Validators {
    /// <summary>
    /// Validates that a property value matches one of a list of allowed ones (domain).
    /// </summary>
    public class DomainValidator : Validator {

        /// <summary>
        /// Validates that a property value matches one of a list of allowed ones (domain).
        /// </summary>
        public DomainValidator(string propertyName, string description) : base(propertyName, description) { }
        public DomainValidator(string[] domain) : this(null, domain) { }
        public DomainValidator(string description, string[] domain) : this(null, description, domain) { }
        public DomainValidator(string propertyName, string description, string[] domain) : base(propertyName, description) {
            Domain = domain;
        }

        public string[] Domain { get; set; }

        public override bool Validate(BusinessObjectBase businessObject) {
            var v = (string)GetPropertyValue(businessObject, PropertyName);
            return string.IsNullOrEmpty(v) || Array.IndexOf(Domain, v) != -1;
        }
    }
}
