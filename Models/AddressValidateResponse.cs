using System.Xml.Serialization;

namespace Response
{
    [XmlRoot(ElementName = "Address")]
    public class Address
    {
        [XmlElement(ElementName = "Address1")]
        public string Address1 { get; set; }

        [XmlElement(ElementName = "Address2")]
        public string Address2 { get; set; }

        [XmlElement(ElementName = "City")]
        public string City { get; set; }

        [XmlElement(ElementName = "CityAbbreviation")]
        public string CityAbbreviation { get; set; }

        [XmlElement(ElementName = "State")]
        public string State { get; set; }

        [XmlElement(ElementName = "Zip5")]
        public string Zip5 { get; set; }

        [XmlElement(ElementName = "Zip4")]
        public string Zip4 { get; set; }

        [XmlElement(ElementName = "DeliveryPoint")]
        public string DeliveryPoint { get; set; }

        [XmlElement(ElementName = "CarrierRoute")]
        public string CarrierRoute { get; set; }

        [XmlElement(ElementName = "Footnotes")]
        public string Footnotes { get; set; }

        [XmlElement(ElementName = "DPVConfirmation")]
        public string? DPVConfirmation { get; set; }

        [XmlElement(ElementName = "DPVCMRA")]
        public string DPVCMRA { get; set; }

        [XmlElement(ElementName = "DPVFootnotes")]
        public string DPVFootnotes { get; set; }

        [XmlElement(ElementName = "Business")]
        public string Business { get; set; }

        [XmlElement(ElementName = "CentralDeliveryPoint")]
        public string CentralDeliveryPoint { get; set; }

        [XmlElement(ElementName = "Vacant")]
        public string Vacant { get; set; }

        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "AddressValidateResponse")]
    public class AddressValidateResponse
    {
        [XmlElement(ElementName = "Address")]
        public Address Address { get; set; } = new Address();
    }


}
