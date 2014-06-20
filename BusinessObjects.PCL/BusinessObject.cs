using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml;
using BusinessObjects.Validators;

namespace BusinessObjects.PCL {
    /// <summary>
    /// The class all domain objects must inherit from. 
    ///
    /// Currently supports:
    /// - XML (de)serialization;
    /// - Exensible and complex validation;
    /// - IEquatable so you can easily compare complex BusinessObjects togheter.
    /// - Binding (INotififyPropertyChanged and IDataErrorInfo).
    /// 
    /// TODO:
    /// - BeginEdit()/EndEdit() combination, and rollbacks for cancels (IEditableObject).
    /// </summary>
    public abstract class BusinessObject:  
        INotifyPropertyChanged,
         IEquatable<BusinessObject> {
        protected List<Validator> Rules;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected BusinessObject() {}
        protected BusinessObject(XmlReader r) : this() { ReadXml(r); }
        //protected BusinessObject(string fileName) : this() { ReadXml(fileName); }

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
                    if (!(v is BusinessObject)) continue;

                    var childDomainObject = (BusinessObject)v;
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
                if (v is BusinessObject && ((BusinessObject) v).IsEmpty()) 
                    i++;
            }
            return i == props.Count();
        }

        /// <summary>
        /// Provides a list of actual data properties for the current BusinessObject instance, sorted by writing order.
        /// </summary>
        /// <remarks>Only properties flagged with the OrderedDataProperty attribute will be returned.</remarks>
        /// <returns>A enumerable list of PropertyInfo instances.</returns>
        protected IEnumerable<PropertyInfo> GetAllDataProperties() {
            var props = GetType().GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(OrderedDataProperty)));
            return props.OrderBy(order => ((OrderedDataProperty)Attribute.GetCustomAttribute(order, typeof(OrderedDataProperty))).Order);
        }

        #region XML

        /// <summary>
        /// The name of the XML Element that is bound to store this BusinessObject instance.
        /// </summary>
        public abstract string XmlName { get; }

        /// <summary>
        /// Optional string format to be applied to DateTime values being serialized to XML.
        /// </summary>
        public virtual string XmlDateFormat { get { return null; } }

        /// <summary>
        /// Array of DateTime properties for which the XmlDateFormat property should be ignored.
        /// </summary>
        public virtual string[] XmlDateFormatIgnoreProperties { get { return null; } }

        /// <summary>
        /// Serializes the current BusinessObject instance to a XML file.
        /// </summary>
        /// <param name="fileName">Name of the file to write to.</param>
        public virtual void WriteXml(string fileName) {
            var settings = new XmlWriterSettings {Indent = true};
            using (var writer = XmlWriter.Create(new System.Text.StringBuilder(fileName), settings)) { WriteXml(writer); }
        }

        /// <summary>
        /// Serializes the current BusinessObject instance to a XML stream.
        /// </summary>
        /// <param name="w">Active XML stream writer.</param>
        /// <remarks>Writes only its inner content, not the outer element. Leaves the writer at the same depth.</remarks>
        public virtual void WriteXml(XmlWriter w) {
            foreach (var prop in GetAllDataProperties())
            {
                var propertyValue = prop.GetValue(this, null);
                if (propertyValue == null) continue;

                // if it's a BusinessObject instance just let it flush it's own data.
                var child = propertyValue as BusinessObject;
                if (child != null) {
                    if (child.IsEmpty()) continue;
                    w.WriteStartElement(child.XmlName);
                    child.WriteXml(w);
                    w.WriteEndElement();
                    continue;
                }

                // if property type is List<T>, assume it's of BusinessObjects and try to fetch them all from XML.
                var tList = typeof (List<>);
                var propertyType = prop.PropertyType;
                if (prop.PropertyType.IsGenericType && tList.IsAssignableFrom(propertyType.GetGenericTypeDefinition()) ||
                    propertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == tList)) {
                    WriteXmlList(propertyValue, w);
                    continue;
                }

                // DateTimes deserve special treatment if XmlDateFormat is set.
                if (propertyValue is DateTime && XmlDateFormat != null) {
                    if (XmlDateFormatIgnoreProperties == null || Array.IndexOf(XmlDateFormatIgnoreProperties, prop.Name) == -1) {
                        w.WriteElementString(prop.Name, ((DateTime)propertyValue).ToString(XmlDateFormat));
                        continue;
                    }
                }
                if (propertyValue is string) {
                    if (!string.IsNullOrEmpty(propertyValue.ToString())) {
                        w.WriteElementString(prop.Name, propertyValue.ToString());
                    }
                    continue;
                }
                if (propertyValue is decimal) {
                    w.WriteElementString(prop.Name, ((decimal)propertyValue).ToString("0.00", CultureInfo.InvariantCulture));
                    continue;
                }

                // all else fail so just let the value flush straight to XML.
                w.WriteStartElement(prop.Name); 
                w.WriteValue(propertyValue); 
                w.WriteEndElement();
            }
        }

        /// <summary>
        /// Deserializes a List of BusinessObject to one or more XML elements.
        /// </summary>
        /// <param name="propertyValue">Property value.</param>
        /// <param name="w">Active XML stream writer.</param>
        private static void WriteXmlList(object propertyValue, XmlWriter w)
        {
            var e = propertyValue.GetType().GetMethod("GetEnumerator").Invoke(propertyValue, null) as IEnumerator;

            while (e != null && e.MoveNext()) {
                var bo = e.Current as BusinessObject;
                // ReSharper disable once PossibleNullReferenceException
                w.WriteStartElement(bo.XmlName);
                bo.WriteXml(w);
                w.WriteEndElement();
            }
        }

        /// <summary>
        /// Deserializes the current BusinessObject from a XML file.
        /// </summary>
        /// <param name="fileName">Name of the file to read from.</param>
        //public virtual void ReadXml(string fileName) {
        //    var settings = new XmlReaderSettings {IgnoreWhitespace = true};
        //    using (var reader = XmlReader.Create(fileName, settings)) { ReadXml(reader); }
        //}

        /// <summary>
        /// Deserializes the current BusinessObject from a XML stream.
        /// </summary>
        /// <param name="r">Active XML stream reader.</param>
        /// <remarks>Reads the outer element. Leaves the reader at the same depth.</remarks>
        // TODO Clear properties before reading from file
        public virtual void ReadXml(XmlReader r) {
            var props = GetAllDataProperties().ToList();
            r.ReadStartElement();
            while (r.NodeType == XmlNodeType.Element) {

                var prop = props.FirstOrDefault(n => n.Name.Equals(r.Name));
                if (prop == null) {
                    // ignore unknown property.
                    r.Skip();
                    continue;
                }

                var propertyType = prop.PropertyType;
                var propertyValue = prop.GetValue(this, null);

                // if property type is BusinessObject, let it auto-load from XML.
                if (typeof(BusinessObject).IsAssignableFrom(propertyType)) {
                    ((BusinessObject)propertyValue).ReadXml(r);
                    continue;
                }

                // if property type is List<T>, assume it's of BusinessObjects and try to fetch them from XML.
                var tList = typeof (List<>);
                if (propertyType.IsGenericType && tList.IsAssignableFrom(propertyType.GetGenericTypeDefinition()) ||
                    propertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == tList)) {
                    ReadXmlList(propertyValue, prop.Name, r);
                    continue;
                }

                prop.SetValue(this, r.ReadElementContentAs(propertyType, null), null);
            }
            r.ReadEndElement();
        }

        /// <summary>
        /// Serializes one or more XML elements into a List of BusinessObjects.
        /// </summary>
        /// <param name="propertyValue">Property value. Must be a List of BusinessObject instances.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="r">Active XML stream reader.</param>
        private static void ReadXmlList(object propertyValue, string propertyName,  XmlReader r) {

            // retrieve type of list elements.
            var elementType = propertyValue.GetType().GetGenericArguments().Single();

            // quit if it's not a BusinessObject subclass.
            if (elementType.BaseType == null) return;
            if (!typeof(BusinessObject).IsAssignableFrom(elementType)) return;

            // clear the list first.
            propertyValue.GetType().GetMethod("Clear").Invoke(propertyValue, null);

            while (r.NodeType == XmlNodeType.Element && r.Name == propertyName) {
                var bo = Activator.CreateInstance(elementType);
                ((BusinessObject)bo).ReadXml(r);
                propertyValue.GetType().GetMethod("Add").Invoke(propertyValue, new[] { bo });
            }
        }
        #endregion

        #region IEquatable
        public bool Equals(BusinessObject other)
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

            var o = obj as BusinessObject;
            return o != null && Equals(o);
        }
        public static bool operator == (BusinessObject o1, BusinessObject o2)
        {
            if ((object)o1 == null || ((object)o2) == null)
                return Equals(o1, o2);

            return o1.Equals(o2);
        }

        public static bool operator != (BusinessObject o1, BusinessObject o2)
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