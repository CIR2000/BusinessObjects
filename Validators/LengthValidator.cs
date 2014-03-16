namespace BusinessObjects.Validators {
    public class LengthValidator : Validator {

        private int _max, _min;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LengthValidator(int length) : this(null, length) { }
        public LengthValidator(string propertyName, int length) : this(propertyName, string.Format("Length must be {0}.", length), length) { }
        public LengthValidator(string propertyName, string description, int length) : this(propertyName, description, length, length) { }
        public LengthValidator(int min, int max) : this(null,  min, max) { }
        public LengthValidator(string propertyName, int min, int max) : this(propertyName, string.Format("Length must be between {0} and {1}.", min, max), min, max) { }
        public LengthValidator(string propertyName, string description, int min, int max) : base(propertyName, description) {
            _max = max;
            _min = min;
        }

        public override bool Validate(BusinessObject businessObject) {
            string v = (string)GetPropertyValue(businessObject);
            return string.IsNullOrEmpty(v) || v.Length >= _min && v.Length <= _max;
        }
    }
}
