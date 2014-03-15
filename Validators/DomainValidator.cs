using System.Reflection;
using System;

namespace BusinessObjects {
    public class DomainValidator : Validator {

        private string[] _domain;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DomainValidator(string[] domain) : this(null, domain) { }
        public DomainValidator(string propertyName, string[] domain) : this(propertyName, "Value not allowed.", domain) { }
        public DomainValidator(string propertyName, string description, string[] domain) : base(propertyName, description) {
            _domain = domain;
        }

        public override bool Validate(DomainObject domainObject) {
            string v = (string)GetPropertyValue(domainObject);
            return Array.IndexOf(_domain, v) != -1;
        }
    }
}
