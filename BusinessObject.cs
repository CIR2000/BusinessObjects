using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using BusinessObjects.Validators;

namespace BusinessObjects {
    /// <summary>
    /// The class all domain objects must inherit from. 
    ///
    /// Currently supports:
    /// - XML (de)serialization;
    /// - Exensible and complex validation;
    /// - Binding (INotififyPropertyChanged and IDataErrorInfo).
    /// 
    /// TODO:
    /// - BeginEdit()/EndEdit() combination, and rollbacks for cancels (IEditableObject).
    /// </summary>
    [Serializable]
    public abstract class BusinessObject:  
        INotifyPropertyChanged,
        IDataErrorInfo {
        protected List<Validator> Rules;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected BusinessObject() {}

        protected BusinessObject(XmlReader r) : this() { ReadXml(r); }

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

                foreach (var prop in GetAllDataProperties()) {
                    // Only operate on BusinessObject types.
                    if (prop.PropertyType.BaseType != GetType().BaseType || prop.PropertyType.BaseType == null)
                        continue;

                    var childDomainObject = (BusinessObject) prop.GetValue(this, null);
                    if (childDomainObject == null) continue;

                    var childErrors = childDomainObject.Error;
                    if (childErrors == null) continue;

                    // Inject child object name into error messages. 
                    // TODO Kind of hacky. Perhaps review the Error system (array?). 
                    // IDataErrorInfo wants a string as return value however.
                    var errors = childErrors.Split(new[] {"\r\n"},
                        StringSplitOptions.RemoveEmptyEntries);
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

            return broken.AsReadOnly();
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
        ///<remarks>This is a paremeterless version which uses .NET 4.5 CallerMemberName to guess the calling function name.</remarks>
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
                var t = prop.PropertyType;
                if (v == null) {
                    i++;
                    break;
                }
                if (t == typeof (string) && string.IsNullOrEmpty((string) v)) {
                    i++;
                    break;
                }
                if (t == typeof (BusinessObject) && ((BusinessObject) v).IsEmpty()) {
                    i++;
                    break;
                }
            }
            return i == props.Count();
        }

        /// <summary>
        /// Provides a list of actual data properties for the current BusinessObject instance.
        /// </summary>
        /// <remarks>Only properties flagged with the OrderedDataProperty attribute will be returned.</remarks>
        /// <returns>A enumerable list of PropertyInfo instances.</returns>
        protected IEnumerable<PropertyInfo> GetAllDataProperties() {
            return GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(OrderedDataProperty)));
        }

        #region XML

        /// <summary>
        /// The name of the XML Element that is bound to store this BusinessObject instance.
        /// </summary>
        public abstract string XmlName { get; }

        /// <summary>
        /// Serializes the current BusinessObject instance to a XML stream.
        /// </summary>
        /// <param name="w">Active XML stream writer.</param>
        /// <remarks>Writes only its inner content, not the outer element. Leaves the writer at the same depth.</remarks>
        public virtual void WriteXml(XmlWriter w) {
            foreach (var prop in GetAllDataProperties()) {
                var v = prop.GetValue(this, null);
                var o = v as BusinessObject;
                if (o != null) {
                    var child = o;
                    if (child.IsEmpty()) continue;
                    w.WriteStartElement(child.XmlName);
                    child.WriteXml(w);
                    w.WriteEndElement();
                }
                else {
                    var value = CleanString((string)v);
                    if (!string.IsNullOrEmpty(value))
                        w.WriteElementString(prop.Name, value);
                }
            }
        }

        /// <summary>
        /// Deserializes the current BusinessObject from a XML stream.
        /// </summary>
        /// <param name="r">Active XML stream reader.</param>
        /// <remarks>Reads the outer element. Leaves the reader at the same depth.</remarks>
        public virtual void ReadXml(XmlReader r) {
            var props = GetAllDataProperties().ToList();
            r.ReadStartElement();
            while (r.NodeType == XmlNodeType.Element) {
                var prop = props.FirstOrDefault(n => n.Name.Equals(r.Name));
                if (prop != null) {
                    var t = prop.PropertyType;
                    if (t.BaseType == typeof(BusinessObject)) {
                        ((BusinessObject)prop.GetValue(this, null)).ReadXml(r);
                    }
                    else {
                        // TODO handle more types.
                        prop.SetValue(this, r.ReadElementContentAsString(prop.Name, string.Empty), null);
                    }
                }
                else {
                    // Ignore unknown element.
                    r.Skip();
                }
            }
            r.ReadEndElement();
        }
        #endregion
    }
}