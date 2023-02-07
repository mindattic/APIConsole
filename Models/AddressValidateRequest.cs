using System.Xml.Serialization;

namespace Request
{
    [XmlRoot(ElementName = "Address")]
    public class Address
    {
        [XmlElement(ElementName = "Address1")]
        public string Address1 { get; set; }

        [XmlElement(ElementName = "Address2")]
        public string Address2 { get; set; }

        [XmlElement(ElementName = "City")]
        public object City { get; set; }

        [XmlElement(ElementName = "State")]
        public string State { get; set; }

        [XmlElement(ElementName = "Zip5")]
        public string Zip5 { get; set; }

        [XmlElement(ElementName = "Zip4")]
        public string Zip4 { get; set; }
    }

    [XmlRoot(ElementName = "AddressValidateRequest")]
    public class AddressValidateRequest
    {
        [XmlElement(ElementName = "Revision")]
        public string Revision { get; set; }

        [XmlElement(ElementName = "Address")]
        public Address Address { get; set; } = new Address();
    }

}
