using System.Xml.Serialization;

namespace Esadad.Infrastructure.DTOs
{
    [XmlRoot(ElementName = "MFEP")]
    public class PrePaidResponseDto
    {
        [XmlElement(ElementName = "MsgHeader")]
        public MsgHeader MsgHeader { get; set; }

        [XmlElement(ElementName = "MsgBody")]
        public PrePaidResponseBody MsgBody { get; set; }

        [XmlElement(ElementName = "MsgFooter")]
        public MsgFooter MsgFooter { get; set; }
    }

    [XmlRoot(ElementName = "MsgBody")]
    public class PrePaidResponseBody
    {
        [XmlElement(ElementName = "BillingInfo")]
        public BillingInfo BillingInfo { get; set; }
    }

    [XmlRoot(ElementName = "BillingInfo")]
    public class BillingInfo
    {
        [XmlElement(ElementName = "Result")]
        public Result Result { get; set; }

        [XmlElement(ElementName = "AcctInfo")]
        public PrepaidAcctInfo AcctInfo { get; set; }

        [XmlElement(ElementName = "DueAmt")]
        public decimal DueAmt { get; set; }

        [XmlElement(ElementName = "Currency")]
        public string Currency { get; set; }

        [XmlElement(ElementName = "ValidationCode")]
        public int ValidationCode { get; set; }

        [XmlElement(ElementName = "ServiceTypeDetails")]
        public ServiceTypeDetails ServiceTypeDetails { get; set; }

        [XmlElement(ElementName = "SubPmts")]
        public SubPmts SubPmts { get; set; }

        [XmlElement(ElementName = "AdditionalInfo")]
        public AdditionalInfo AdditionalInfo { get; set; }
    }
}
