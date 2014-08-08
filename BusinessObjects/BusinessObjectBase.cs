using BusinessObjects.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BusinessObjects {
    /// <summary>
    /// The class all domain objects must inherit from. 
    ///
    /// Currently supports:
    /// - Exensible and complex validation;
    /// - IEquatable so you can easily compare complex BusinessObjects togheter.
    /// - Binding (INotififyPropertyChanged and IDataErrorInfo).
    /// 
    /// TODO:
    /// - BeginEdit()/EndEdit() combination, and rollbacks for cancels (IEditableObject).
    /// </summary>
    public abstract class BusinessObjectBase:  
        INotifyPropertyChanged,
        IEquatable<BusinessObjectBase> {
        protected List<Validator> Rules;

        /// <summary>
        /// Gets a value indicating whether or not this domain object is valid. 
        /// </summary>
        public virtual bool IsValid {
            get {
                return Error == null;
            }
        }
        /// <summary>
        /// Gets an error message indicating what is wrong with this domain object. The default is a null string.
        /// </summary>
        public virtual string Error {
            get
            {
                // Get object errors
                var result = this[string.Empty];
                // Also retrieve child objects errors
                var childrenErrors = ChildrenErrors;

                result += (result != null) ? Environment.NewLine + childrenErrors : childrenErrors;
                if (result.Trim().Length == 0) {
                    result = null;
                }
                return result;
            }
        }

        /// <summary>
        /// Gets error messages indicating what is wrong with eventual child domain objects. The default is a null string.
        /// </summary>
        private string ChildrenErrors {
            get {
                string result = null;

                foreach (var prop in GetAllDataProperties())
                {
                    var v = prop.GetValue(this, null);

                    // Only operate on BusinessObject types.
                    if (!(v is BusinessObjectBase)) continue;

                    var childDomainObject = (BusinessObjectBase)v;
                    if (childDomainObject.IsEmpty()) continue;

                    var childErrors = childDomainObject.Error;
                    if (childErrors == null) continue;

                    // TODO Kind of hacky. Perhaps review the Error system (array?). 
                    // Inject child object name into error messages. 
                    // IDataErrorInfo wants a string as return value however.
                    var errors = childErrors.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var error in errors) {
                        result += prop.Name + "." + error + Environment.NewLine;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The name of the property whose error message to get.</param>
        /// <returns>The error message for the property. The default is an empty string ("").</returns>
        public virtual string this[string propertyName] {
            get {
                var result = string.Empty;

                foreach (var validator in GetBrokenRules(propertyName)) {
                    if (propertyName == string.Empty || validator.PropertyName == propertyName) {
                        result += propertyName + validator.PropertyName + ": " + validator.Description;
                        result += Environment.NewLine;
                    }
                }
                result = result.Trim();
                if (result.Length == 0) {
                    result = null;
                }
                return result;
            }
        }

        /// <summary>
        /// Validates all rules on this domain object, returning a list of the broken rules.
        /// </summary>
        /// <returns>A read-only collection of rules that have been broken.</returns>
        public virtual ReadOnlyCollection<Validator> GetBrokenRules() {
            return GetBrokenRules(string.Empty);
        }

        /// <summary>
        /// Validates all rules on this domain object for a given property, returning a list of the broken rules.
        /// </summary>
        /// <param name="property">The name of the property to check for. If null or empty, all rules will be checked.</param>
        /// <returns>A read-only collection of rules that have been broken.</returns>
        public virtual ReadOnlyCollection<Validator> GetBrokenRules(string property) {
            property = CleanString(property);
            
            // If we haven't yet created the rules, create them now.
            if (Rules == null) {
                Rules = new List<Validator>();
                Rules.AddRange(CreateRules());
            }
            var broken = new List<Validator>();

            
            foreach (var validator in Rules) {
                // Ensure we only validate a rule 
                if (validator.PropertyName == property || property == string.Empty) {
                    var isRuleBroken = !validator.Validate(this);
                    //Debug.WriteLine(DateTime.Now.ToLongTimeString() + ": Validating the rule: '" + r.ToString() + "' on object '" + this.ToString() + "'. Result = " + ((isRuleBroken == false) ? "Valid" : "Broken"));
                    if (isRuleBroken) {
                        broken.Add(validator);
                    }
                }
            }
            return new ReadOnlyCollection<Validator>(broken);
        }

        /// <summary>
        /// Occurs when any properties are changed on this object.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Override this method to create your own rules to validate this business object. These rules must all be met before 
        /// the business object is considered valid enough to save to the data store.
        /// </summary>
        /// <returns>A collection of rules to add for this business object.</returns>
        protected virtual List<Validator> CreateRules() {
            return new List<Validator>();
        }

        /// <summary>
        /// A helper method that raises the PropertyChanged event for a property.
        /// </summary>
        ///<remarks>This is a paremeterless version which uses .NET 4.0 CallerMemberName to guess the calling function name.</remarks>
        protected virtual void NotifyChanged([CallerMemberName] string caller = "") {
            NotifyChanged(new[]{caller});
        }
        /// <summary>
        /// A helper method that raises the PropertyChanged event for a property.
        /// </summary>
        /// <param name="propertyNames">The names of the properties that changed.</param>
        /// <remarks>This is a .NET 2.0 compatible version.</remarks>
        protected virtual void NotifyChanged(params string[] propertyNames) {
            foreach (var name in propertyNames) {
                OnPropertyChanged(new PropertyChangedEventArgs(name));
            }
            OnPropertyChanged(new PropertyChangedEventArgs("IsValid"));
        }

        /// <summary>
        /// Cleans a string by ensuring it isn't null and trimming it.
        /// </summary>
        /// <param name="s">The string to clean.</param>
        protected string CleanString(string s) {
            return (s ?? string.Empty).Trim();
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
            if (PropertyChanged != null) {
                PropertyChanged(this, e);
            }
        }

        /// <summary>
        /// Checks wether a BusinessObject instance is empty.
        /// </summary>
        /// <returns>Returns true if the object is empty; false otherwise.</returns>
        public virtual Boolean IsEmpty()
        {
            // TODO support more data types.

            var props = GetAllDataProperties().ToList();
            var i = 0;
            foreach (var prop in props) {
                var v = prop.GetValue(this, null);
                if (v == null) {
                    i++;
                    continue;
                }
                if (v is string) {
                    if (string.IsNullOrEmpty((string) v)) 
                        i++;
                    continue;
                }
                if (v is BusinessObjectBase && ((BusinessObjectBase)v).IsEmpty()) { 
                    i++;
                }
            }
            return i == props.Count();
        }

        /// <summary>
        /// Provides a list of actual data properties for the current BusinessObject instance, sorted by writing order.
        /// </summary>
        /// <remarks>Only properties flagged with the OrderedDataProperty attribute will be returned.</remarks>
        /// <returns>A enumerable list of PropertyInfo instances.</returns>
        protected IEnumerable<PropertyInfo> GetAllDataProperties() {
            var props = GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(DataProperty)));
            return props.OrderBy(order => ((DataProperty)Attribute.GetCustomAttribute(order, typeof(DataProperty))).Order);
        }


        #region IEquatable
        public bool Equals(BusinessObjectBase other)
        {
            if (other == null)
                return false;

            foreach (var prop in GetAllDataProperties()) {
                var v1 = prop.GetValue(this, null);
                var v2 = prop.GetValue(other, null);
                if ( v1 != v2 && !v1.Equals(v2)) {
                    return false;
                }
            }
            return true;
        }
        public override bool Equals(object obj) {
            if (obj == null)
                return false;

            var o = obj as BusinessObjectBase;
            return o != null && GetType().Name == o.GetType().Name && Equals(o);
        }
        public static bool operator == (BusinessObjectBase o1, BusinessObjectBase o2)
        {
            if ((object)o1 == null || ((object)o2) == null)
                return Equals(o1, o2);

            return o1.Equals(o2);
        }

        public static bool operator != (BusinessObjectBase o1, BusinessObjectBase o2)
        {
            if (o1 == null || o2 == null)
                return !Equals(o1, o2);

            return !(o1.Equals(o2));
        }
        public override int GetHashCode() {
            return this.GetHashCodeFromFields(GetAllDataProperties());
        }
        #endregion
    }
    public static class ObjectExtensions
    {
        private const int SeedPrimeNumber = 691;
        private const int FieldPrimeNumber = 397;
        /// <summary>
        /// Allows GetHashCode() method to return a Hash based ont he object properties.
        /// </summary>
        /// <param name="obj">The object fro which the hash is being generated.</param>
        /// <param name="fields">The list of fields to include in the hash generation.</param>
        /// <returns></returns>
        public static int GetHashCodeFromFields(this object obj, params object[] fields)
        {
            unchecked
            { //unchecked to prevent throwing overflow exception
                var hashCode = SeedPrimeNumber;
                foreach (var b in fields)
                    if (b != null)
                        hashCode *= FieldPrimeNumber + b.GetHashCode();
                return hashCode;
            }
        }
    }
}