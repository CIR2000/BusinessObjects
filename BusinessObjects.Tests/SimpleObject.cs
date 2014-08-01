using System.Xml;

namespace BusinessObjects.Tests
{
    public class SimpleObject : BusinessObject {
        public SimpleObject() { }
        public SimpleObject(XmlReader r) : base(r) { }

        [DataProperty (order:1)] 
        public string SimpleProperty { get; set; }

        [DataProperty (order:0)]
        public string AnotherProperty { get; set; }
    }
}