using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Esadad.Infrastructure.DTOs
{
    [Serializable]
    [XmlRoot(ElementName = "MFEP")]
    public class PaymentNotificationRequestDto
    {
        public RequestMsgHeader MsgHeader { get; set; }
        public PaymNotificationRequestRequestMsgBody MsgBody { get; set; }
        public MsgFooter MsgFooter { get; set; }
    }

    [Serializable]
    [XmlRoot(ElementName = "MsgBody")]
    public class PaymNotificationRequestRequestMsgBody
    {
        public PaymNotificationRequestTransactions Transactions { get; set; }
    }
    [Serializable]
    public class PaymNotificationRequestTransactions
    {
        public PaymNotificationRequestTrxInf TrxInf { get; set; }
    }
    [Serializable]
    public class PaymNotificationRequestTrxInf
    {
        public AcctInfo AcctInfo { get; set; }
        public long JOEBPPSTrx { get; set; } // Mandatory, Integer, Long
        public string BankTrxID { get; set; } // Mandatory, String, No length restriction provided
        public int BankCode { get; set; } // Mandatory, Integer, Up to 3 digits
        public string PmtStatus { get; set; } // Optional, String
        public decimal DueAmt { get; set; } // Mandatory, Decimal, Up to (12,3)
        public decimal PaidAmt { get; set; } // Mandatory, Decimal, Up to (12,3)
        public decimal FeesAmt { get; set; } // Mandatory, Decimal, Up to (12,3)
        public bool FeesOnBiller { get; set; } // Boolean, Yes/No
        public DateTime ProcessDate { get; set; } // Mandatory
        public DateTime STMTDate { get; set; } // Mandatory
        public string AccessChannel { get; set; } // Optional, Enum, Up to 15 chars
        public string PaymentMethod { get; set; } // Optional, String, Up to 15 chars
        public string PaymentType { get; set; } // Optional, String, Up to 15 chars
        public string Currency { get; set; } // Optional, String, Up to 3 chars
        public ServiceTypeDetails ServiceTypeDetails { get; set; }
        public SubPmts SubPmts { get; set; }
    }
}
