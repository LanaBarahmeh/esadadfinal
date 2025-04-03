using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Esadad.Infrastructure.DTOs
{
    [Serializable]
    [XmlRoot("MFEP")]
    public class BillPullRequest
    {
        [XmlElement(ElementName = "MsgHeader")]
        public RequestMsgHeader MsgHeader { get; set; }

        [XmlElement(ElementName = "MsgBody")]
        public MsgBody MsgBody { get; set; }

        [XmlElement(ElementName = "MsgFooter")]

        public MsgFooter MsgFooter { get; set; }
    }

    [Serializable]
    public class MsgBody
    {
        [XmlElement("AcctInfo")]
        public AcctInfo AcctInfo { get; set; }

        [XmlElement("ServiceType")]
        public string ServiceType { get; set; }

        [XmlElement("PayerInfo")]
        public PayerInfo PayerInfo { get; set; }
    }
    //[Serializable]
    //public class PayerInfo
    //{
    //    [XmlElement("IdType")]
    //    public string IdType { get; set; }

    //    [XmlElement("Id")]
    //    public string Id { get; set; }

    //    [XmlElement("Nation")]
    //    public string Nation { get; set; }

    //   [XmlElement("Name")]
    //    public string Name { get; set; }

    //    [XmlElement("Phone")]
    //    public string Phone { get; set; }

    //   [XmlElement("Address")]
    //    public string Address { get; set; }

    //    [XmlElement("Email")]
    //    public string Email { get; set; }

    //    [XmlElement("JOEBPPSNo")]
    //    public int JOEBPPSNo { get; set; }
    //}

}
