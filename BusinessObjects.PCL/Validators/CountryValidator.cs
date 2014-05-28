using BusinessObjects.PCL;

namespace BusinessObjects.Validators {
    /// <summary>
    /// Validates that a property conforms to ISO 3166-1 alpha 2 codes.
    /// </summary>
    public class CountryValidator : DomainValidator {

        /// <summary>
        /// Validates that a property conforms to ISO 3166-1 alpha 2 codes.
        /// </summary>
        public CountryValidator(string propertyName) : this(propertyName, "Must be a ISO 3166-1 alpha 2 code ([IT], [UK], [...])") { }

        public CountryValidator(string propertyName, string description) : base (propertyName, description) {
            Domain = Country.TwoLetterCodes;
        }

    }
}
