namespace BusinessObjects.Validators {
    /// <summary>
    /// A simple type of domain object rule that uses a delegate for validation. 
    /// </summary>
    /// <returns>True if the rule has been followed, or false if it has been broken.</returns>
    /// <remarks>
    /// Usage:
    /// <code>
    ///     this.Rules.Add(new SimpleRule("Name", "The customer name must be at least 5 letters long.", delegate { return this.Name &gt; 5; } ));
    /// </code>
    /// </remarks>
    public delegate bool SimpleValidatorDelegate();

    /// <summary>
    /// A class to define a simple rule, using a delegate for validation.
    /// </summary>
    public class DelegateValidator : 
        Validator {
        private SimpleValidatorDelegate _validatorDelegate;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyName">The name of the property this rule validates for. This may be blank.</param>
        /// <param name="brokenDescription">A description message to show if the rule has been broken.</param>
        /// <param name="ruleDelegate">A delegate that takes no parameters and returns a boolean value, used to validate the rule.</param>
        public DelegateValidator(string propertyName, string description, SimpleValidatorDelegate ruleDelegate):
            base(propertyName, description) {
            this.ValidatorDelegate = ruleDelegate;
        }

        /// <summary>
        /// Gets or sets the delegate used to validate this rule.
        /// </summary>
        protected virtual SimpleValidatorDelegate ValidatorDelegate {
            get { return _validatorDelegate; }
            set { _validatorDelegate = value; }
        }

        /// <summary>
        /// Validates that the rule has not been broken.
        /// </summary>
        /// <param name="domainObject">The domain object being validated.</param>
        /// <returns>True if the rule has not been broken, or false if it has.</returns>
        public override bool Validate(DomainObject domainObject) {
            return ValidatorDelegate();
        }
    }
}
