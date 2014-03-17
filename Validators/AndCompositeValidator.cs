using System.Collections.Generic;
using System.Linq;

namespace BusinessObjects.Validators {
    public class AndCompositeValidator : Validator {

        private readonly List<Validator> _validators;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AndCompositeValidator(string propertyName, List<Validator> validators) : base(propertyName, null) {
            _validators = validators;
            foreach (var v in _validators) {
                v.PropertyName = propertyName;
            }
        }

        /// <summary>
        /// Validates that the rule has been followed.
        /// </summary>
        /// <remarks>Description will only express broken validation rules, or null.</remarks>
        public override bool Validate(BusinessObject businessObject) {
            var result = true;
            Description = null;
            foreach (var v in _validators.Where(v => !v.Validate(businessObject)))
            {
                Description += v.Description;
                result = false;
            }
            return result;
        }
    }
}
