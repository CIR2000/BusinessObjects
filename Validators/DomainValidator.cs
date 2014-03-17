using System;

namespace BusinessObjects.Validators {
    public class DomainValidator : Validator {

        private readonly string[] _domain;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DomainValidator(string[] domain) : this(null, domain) { }
        public DomainValidator(string propertyName, string[] domain) : this(propertyName, "Value not allowed.", domain) { }
        public DomainValidator(string propertyName, string description, string[] domain) : base(propertyName, description) {
            _domain = domain;
        }

        public override bool Validate(BusinessObject businessObject) {
            var v = (string)GetPropertyValue(businessObject);
            return Array.IndexOf(_domain, v) != -1;
        }
    }
}
