using System.Collections.Generic;

namespace BusinessObjects.Validators {
    public class AndCompositeValidator : Validator {

        private List<Validator> _validators;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AndCompositeValidator(string propertyName, List<Validator> validators) : base(propertyName, null) {
            _validators = validators;
            foreach (Validator v in _validators) {
                v.PropertyName = propertyName;
            }
        }

        /// <summary>
        /// Validates that the rule has been followed.
        /// </summary>
        /// <remarks>Description will only express broken validation rules, or null.</remarks>
        public override bool Validate(BusinessObject businessObject) {
            bool _result = true;
            this.Description = null;
            foreach (Validator v in _validators) {
                if (!v.Validate(businessObject)) {
                    this.Description += v.Description;
                    _result = false;
                }
            }
            return _result;
        }
    }
}
