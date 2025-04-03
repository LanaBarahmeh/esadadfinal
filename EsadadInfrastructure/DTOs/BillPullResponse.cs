using System.Xml.Serialization;

namespace Esadad.Infrastructure.DTOs
{
    [XmlRoot(ElementName = "MFEP")]
    public class BillPullResponse
    {
        [XmlElement(ElementName = "MsgHeader")]
        public MsgHeader MsgHeader { get; set; }

        [XmlElement(ElementName = "MsgBody")]
        public BillPullResponseBody MsgBody { get; set; }

        [XmlElement(ElementName = "MsgFooter")]
        public MsgFooter MsgFooter { get; set; }
    }

    [XmlRoot(ElementName = "MsgBody")]
    public class BillPullResponseBody
    {
        [XmlElement(ElementName = "RecCount")]
        public int RecCount { get; set; }

        [XmlElement(ElementName = "BillsRec")]
        public BillsRec BillsRec { get; set; }
    }

    public class BillsRec
    {
        [XmlElement(ElementName = "BillRec")]
        public BillRec BillRec { get; set; }
    }

    public class BillRec
    {
        [XmlElement(ElementName = "Result")]
        public Result Result { get; set; }

        [XmlElement(ElementName = "AcctInfo")]
        public AcctInfo AcctInfo { get; set; }

        [XmlElement(ElementName = "BillStatus")]
        public string BillStatus { get; set; }

        [XmlElement(ElementName = "DueAmount")]
        public decimal DueAmount { get; set; }

        [XmlElement(ElementName = "Currency")]
        public string Currency { get; set; }

        [XmlElement(ElementName = "IssueDate")]
        public DateTime IssueDate { get; set; }

        [XmlElement(ElementName = "DueDate")]
        public DateTime DueDate { get; set; }

        [XmlElement(ElementName = "ServiceType")]
        public string ServiceType { get; set; }

        [XmlElement(ElementName = "PmtConst")]
        public PmtConst PmtConst { get; set; }

        [XmlElement(ElementName = "SubPmts")]
        public SubPmts SubPmts { get; set; }

        [XmlElement(ElementName = "AdditionalInfo")]
        public AdditionalInfo AdditionalInfo { get; set; }
    }

    public class PmtConst
    {
        [XmlElement(ElementName = "AllowPart")]
        public bool AllowPart { get; set; }

        [XmlElement(ElementName = "Lower")]
        public decimal Lower { get; set; }

        [XmlElement(ElementName = "Upper")]
        public decimal Upper { get; set; }
    }


}