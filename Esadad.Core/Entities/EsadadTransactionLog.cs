using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esadad.Core.Entities
{
    [Table("ESADAD_TRANSACTIONS_LOGS")]
    public class EsadadTransactionLog
    {
        [Key]
        [Column("ID")]
        public long Id { get; set; }

        [Required]
        [Column("TRANSACTION_TYPE", TypeName = "VARCHAR2(50)")]
        public string TransactionType { get; set; }

        [Required]
        [Column("API_NAME", TypeName = "VARCHAR2(50)")]
        public string ApiName { get; set; }

        [Required]
        [Column("GUID", TypeName = "VARCHAR2(50)")]
        public string Guid { get; set; }

        [Required]
        [Column("TIMESTAMP", TypeName = "DATE")]
        public DateTime Timestamp { get; set; }

        [Column("BILLING_NUMBER", TypeName = "NVARCHAR2(50)")]
        public string BillingNumber { get; set; }

        [Column("BILL_NUMBER", TypeName = "NVARCHAR2(50)")]
        public string BillNumber { get; set; }

        [Column("CURRENCY", TypeName = "VARCHAR2(10)")]
        public string Currency { get; set; }

        [Column("SERVICE_TYPE", TypeName = "NVARCHAR2(50)")]
        public string ServiceType { get; set; }

        [Column("PREPAID_CAT", TypeName = "NVARCHAR2(50)")]
        public string PrepaidCat { get; set; }

        [Column("VALIDATION_CODE", TypeName = "VARCHAR2(50)")]
        public string ValidationCode { get; set; }

        [Required]
        [Column("TRAN_XML_ELEMENT", TypeName = "CLOB")]
        public string TranXmlElement { get; set; }

        [Required]
        [Column("INSERT_DATE", TypeName = "DATE")]
        public DateTime InsertDate { get; set; } = DateTime.Now;
    }
}
