using System;
using System.Security.Cryptography;

namespace BusinessObjects.Validators {
    public class DomainValidator : Validator {

        /// <summary>
        /// Constructor.
        /// </summary>
        public DomainValidator(string propertyName, string description) : base(propertyName, description) { }
        public DomainValidator(string[] domain) : this(null, domain) { }
        public DomainValidator(string description, string[] domain) : this(null, description, domain) { }
        public DomainValidator(string propertyName, string description, string[] domain) : base(propertyName, description) {
            Domain = domain;
        }

        public string[] Domain { get; set; }

        public override bool Validate(BusinessObject businessObject) {
            var v = (string)GetPropertyValue(businessObject, PropertyName);
            return Array.IndexOf(Domain, v) != -1;
        }
    }
}
