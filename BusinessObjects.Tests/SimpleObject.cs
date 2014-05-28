using System.Xml;
using BusinessObjects.PCL;

namespace BusinessObjects.Tests
{
    public class SimpleObject : BusinessObject {
        public SimpleObject() { }
        public SimpleObject(XmlReader r) : base(r) { }

        public override string XmlName { get { return "SimpleObject"; } }

        [OrderedDataProperty]
        public string SimpleProperty { get; set; }

        [OrderedDataProperty]
        public string AnotherProperty { get; set; }
    }
}