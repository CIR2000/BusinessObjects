using System.Xml;
using BusinessObjects;

namespace BusinessObjects.Tests
{
    public class SimpleObject : BusinessObject {
        public SimpleObject() { }
        public SimpleObject(XmlReader r) : base(r) { }

        [DataProperty]
        public string SimpleProperty { get; set; }

        [DataProperty]
        public string AnotherProperty { get; set; }
    }
}