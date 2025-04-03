using System.Xml.Serialization;

namespace Esadad.Infrastructure.DTOs
{
    [Serializable]
    public class MsgHeader
    {
        [XmlElement(ElementName = "TmStp")]
        public DateTime TmStp { get; set; }

        [XmlElement(ElementName = "GUID")]
        public Guid GUID { get; set; }

        [XmlElement(ElementName = "TrsInf")]
        public TrsInf TrsInf { get; set; }

        [XmlElement(ElementName = "Result")]
        public Result Result { get; set; }
    }
    [Serializable]
    public class TrsInf
    {
        [XmlElement(ElementName = "SdrCode")]
        public int SdrCode { get; set; }

        [XmlElement(ElementName = "ResTyp")]
        public string ResTyp { get; set; }
    }

    [Serializable]
    public class MsgFooter
    {
        [XmlElement(ElementName = "Security")]
        public Security Security { get; set; }
    }

    [Serializable]
    public class Security
    {
        [XmlElement(ElementName = "Signature")]
        public string Signature { get; set; }
    }
    [Serializable]
    public class Result
    {
        [XmlElement(ElementName = "ErrorCode")]
        public int ErrorCode { get; set; }

        [XmlElement(ElementName = "ErrorDesc")]
        public string ErrorDesc { get; set; }

        [XmlElement(ElementName = "Severity")]
        public string Severity { get; set; }
    }

    [Serializable]
    public class AcctInfo
    {
        [XmlElement(ElementName = "BillingNo")]
        public string BillingNo { get; set; }

        [XmlElement(ElementName = "BillNo")]
        public string BillNo { get; set; }
    }
    [Serializable]
    public class SubPmts
    {
        [XmlElement(ElementName = "SubPmt")]
        public SubPmt SubPmt { get; set; }
    }
    [Serializable]
    public class SubPmt
    {
        [XmlElement("Amount")]
        public decimal Amount { get; set; }

        [XmlElement("SetBnkCode")]
        public int SetBnkCode { get; set; }

        [XmlElement("AcctNo")]
        public string AcctNo { get; set; }
    }

    [XmlRoot(ElementName = "ServiceTypeDetails")]
    [Serializable]
    public class ServiceTypeDetails
    {
        [XmlElement(ElementName = "ServiceType")]
        public string ServiceType { get; set; }

        [XmlElement(ElementName = "PrepaidCat")]
        public string PrepaidCat { get; set; }
    }

    [XmlRoot(ElementName = "AdditionalInfo")]
    [Serializable]
    public class AdditionalInfo
    {
        [XmlElement(ElementName = "CustName")]
        public string CustName { get; set; }

        [XmlElement(ElementName = "FreeText")]
        public string FreeText { get; set; }
    }



    #region Request 
    [Serializable]
    public class RequestMsgHeader
    {
        [XmlElement(ElementName = "TmStp")]
        public DateTime TmStp { get; set; }

        [XmlElement(ElementName = "TrsInf")]
        public RequestTrsInf TrsInf { get; set; }
    }
    [Serializable]
    public class RequestTrsInf
    {
        [XmlElement(ElementName = "SdrCode")]
        public int SdrCode { get; set; }

        [XmlElement(ElementName = "RcvCode")]
        public int RcvCode { get; set; } // Mandatory, Integer, Up to 2 digits

        [XmlElement(ElementName = "ReqTyp")]
        public string ReqTyp { get; set; } // Mandatory, String, Up to 15 chars

    }

    [Serializable]
    public class PayerInfo
    {
        [XmlElement("IdType")]
        public string IdType { get; set; }

        [XmlElement("Id")]
        public string Id { get; set; }

        [XmlElement("Nation")]
        public string Nation { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Phone")]
        public string Phone { get; set; }

        [XmlElement("Address")]
        public string Address { get; set; }

        [XmlElement("Email")]
        public string Email { get; set; }

        [XmlElement("JOEBPPSNo")]
        public int JOEBPPSNo { get; set; }
    }
    #endregion

    #region Prepaid
    [Serializable]
    public class PrepaidAcctInfo
    {
        [XmlElement(ElementName = "BillingNo")]
        public string BillingNo { get; set; }

        [XmlElement(ElementName = "BillerCode")]
        public int BillerCode { get; set; }
    }
    #endregion
}