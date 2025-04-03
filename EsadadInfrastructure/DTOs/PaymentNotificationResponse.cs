using System.Xml.Serialization;

namespace Esadad.Infrastructure.DTOs
{
    [XmlRoot(ElementName = "MFEP")]
    public class PaymentNotificationResponseDto
    {
        [XmlElement(ElementName = "MsgHeader")]
        public MsgHeader MsgHeader { get; set; }

        [XmlElement(ElementName = "MsgBody")]
        public PaymentNotificationResponseBody MsgBody { get; set; }

        [XmlElement(ElementName = "MsgFooter")]
        public MsgFooter MsgFooter { get; set; }
    }

    [XmlRoot(ElementName = "MsgBody")]
    public class PaymentNotificationResponseBody
    {
        [XmlElement(ElementName = "Transactions")]
        public PaymentNotificationResponseTransactions Transactions { get; set; }
    }

    [XmlRoot(ElementName = "Transactions")]
    public class PaymentNotificationResponseTransactions
    {
        [XmlElement(ElementName = "TrxInf")]
        public PaymentNotificationResponseTrxInf TrxInf { get; set; }
    }

    [XmlRoot(ElementName = "TrxInf")]
    public class PaymentNotificationResponseTrxInf
    {
        [XmlElement(ElementName = "JOEBPPSTrx")]
        public string JOEBPPSTrx { get; set; }

        [XmlElement(ElementName = "ProcessDate")]
        public DateTime ProcessDate { get; set; }

        [XmlElement(ElementName = "STMTDate")]
        public string STMTDate { get; set; }

        [XmlElement(ElementName = "Result")]
        public Result Result { get; set; }
    }
}