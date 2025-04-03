using System.Xml.Serialization;

namespace Esadad.Infrastructure.DTOs
{
    [XmlRoot(ElementName = "MFEP")]
    public class PrePaidRequestDto
    {
        [XmlElement(ElementName = "MsgHeader")]
        public PrepaidMsgHeader MsgHeader { get; set; }

        [XmlElement(ElementName = "MsgBody")]
        public PrePaidRequestBody MsgBody { get; set; }

        [XmlElement(ElementName = "MsgFooter")]
        public MsgFooter MsgFooter { get; set; }
    }

    [XmlRoot(ElementName = "MsgBody")]
    public class PrePaidRequestBody
    {
        [XmlElement(ElementName = "BillingInfo")]
        public PrepaidRequestBillingInfo BillingInfo { get; set; }

  
    }

    [XmlRoot(ElementName = "BillingInfo")]
    public class PrepaidRequestBillingInfo
    {
        [XmlElement(ElementName = "AcctInfo")]
        public PrepaidAcctInfo AcctInfo { get; set; }


        [XmlElement(ElementName = "ValidationCode")]
        public int ValidationCode { get; set; }

        [XmlElement(ElementName = "ServiceTypeDetails")]
        public ServiceTypeDetails ServiceTypeDetails { get; set; }

        [XmlElement("PayerInfo")]
        public PayerInfo PayerInfo { get; set; }
    }



    [Serializable]
    public class PrepaidMsgHeader
    {
        [XmlElement(ElementName = "TmStp")]
        public DateTime TmStp { get; set; }

        [XmlElement(ElementName = "GUID")]
        public Guid GUID { get; set; }

        [XmlElement(ElementName = "TrsInf")]
        public PrepaidTrsInf TrsInf { get; set; }
    }

    [Serializable]
    public class PrepaidTrsInf
    {
        [XmlElement(ElementName = "RcvCode")]
        public int RcvCode { get; set; }

        [XmlElement(ElementName = "ReqTyp")]
        public string ReqTyp { get; set; }
    }



}
