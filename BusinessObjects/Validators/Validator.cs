using System.Linq;
using BusinessObjects;

namespace BusinessObjects.Validators {
    /// <summary>
    /// An abstract class that contains information about a rule as well as a method to validate it.
    /// </summary>
    /// <remarks>
    /// This class is primarily designed to be used on a domain object to validate a business rule. In most cases, you will want to use the 
    /// concrete class SimpleRule, which just needs you to supply a delegate used for validation. For custom, complex business rules, you can 
    /// extend this class and provide your own method to validate the rule.
    /// </remarks>
    public abstract class Validator
    {
        private string _propertyName;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected Validator() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyName">The name of the property the rule is based on. This may be blank if the rule is not for any specific property.</param>
        /// <param name="description">A description of the rule that will be shown if the rule is broken.</param>
        protected Validator(string propertyName, string description) {
            PropertyName = propertyName;
            Description = description;
        }

        protected Validator(string[] properties, string description) {
            Properties = properties;
            Description = description;
        }

        protected Validator(string[] properties) {
            Properties = properties;
        }

        protected Validator(string description) {
            Description = description;
        }

        /// <summary>
        /// Gets descriptive text about this broken rule.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the name of the property or properties the rule belongs to.
        /// </summary>
        public string PropertyName {
            get {
                if (Properties == null) return _propertyName;
                var r = Properties.Aggregate(string.Empty, (current, prop) => current + (prop + ", ")); 
                return "[" + r.TrimEnd(new[] {' ', ','}) + "]";
            }
            internal set { _propertyName = value; } 
        }

        /// <summary>
        /// Gets the names of the properties the rule belongs to.
        /// </summary>
        public string[] Properties { get; internal set; }

        /// <summary>
        /// Validates that the rule has been followed.
        /// </summary>
        public abstract bool Validate(BusinessObjectBase businessObject);

        /// <summary>
        /// Gets a string representation of this rule.
        /// </summary>
        /// <returns>A string containing the description of the rule.</returns>
        public override string ToString() {
            return Description;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. System.Object.GetHashCode()
        /// is suitable for use in hashing algorithms and data structures like a hash
        /// table.
        /// </summary>
        /// <returns>A hash code for the current rule.</returns>
        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Given a BusinessObeject property name, returnes its value.
        /// </summary>
        /// <param name="businessObject">A BusinessObject instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>A property value.</returns>
        protected object GetPropertyValue(BusinessObjectBase businessObject, string propertyName) {
            var pi = businessObject.GetType().GetProperty(propertyName);
            return pi.GetValue(businessObject, null);

        }
    }
}
