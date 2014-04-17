namespace BusinessObjects.Tests
{
    public class SimpleObject : BusinessObject {
        public override string XmlName { get { return "SimpleObject"; } }

        [OrderedDataProperty]
        public string SimpleProperty { get; set; }

        [OrderedDataProperty]
        public string AnotherProperty { get; set; }
    }
}