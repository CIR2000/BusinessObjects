using System.Xml;
using BusinessObjects;

namespace BusinessObjects.Tests
{
    public class SimpleObject : BusinessObject {
        public SimpleObject() { }
        public SimpleObject(XmlReader r) : base(r) { }

        [OrderedDataProperty]
        public string SimpleProperty { get; set; }

        [OrderedDataProperty]
        public string AnotherProperty { get; set; }
    }
}