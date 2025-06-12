using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esadad.Core.Entities
{
    [Table("EsadadPaymentsLogs")]
    public class EsadadPaymentLog
    {
        [Key]
      
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Guid { get; set; }

        [Required]
        [StringLength(50)]
        public string BillingNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string BillNumber { get; set; }

        [Required]
        [Column(TypeName = "decimal(12, 3)")]
        public decimal PaidAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string JOEBPPSTrx { get; set; }

        [Required]
        [StringLength(50)]
        public string BankTrxID { get; set; }

        [Required]
        public int BankCode { get; set; }

        [Required]
        [Column(TypeName = "decimal(12, 3)")]
        public decimal DueAmt { get; set; }

        [Required]
        [Column(TypeName = "decimal(12, 3)")]
        public decimal PaidAmt { get; set; }

        [Column(TypeName = "decimal(12, 3)")]
        public decimal? FeesAmt { get; set; }

        [Required]
        [Column(TypeName = "NUMBER(1)")]
        public bool FeesOnBiller { get; set; } = false;

        [Required]
        public DateTime ProcessDate { get; set; }

        [Required]
        public DateTime STMTDate { get; set; }

        [Required]
        [StringLength(50)]
        public string AccessChannel { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentType { get; set; }

        [Required]
        [StringLength(10)]
        public string Currency { get; set; }

        [Required]
        [StringLength(50)]
        public string ServiceType { get; set; }

        [StringLength(50)]
        public string PrepaidCat { get; set; }

        [Required]
        [Column(TypeName = "decimal(12, 3)")]
        public decimal Amount { get; set; }

        [Required]
        public int SetBnkCode { get; set; }

        [Required]
        [StringLength(50)]
        public string AcctNo { get; set; }

        [Required]
        [Column(TypeName = "NUMBER(1)")]
        public bool IsPaymentPosted { get; set; }

        //[Required]
        //[Column(TypeName = "datetime")]
        //public DateTime InsertDate { get; set; }= DateTime.Now;
       
    }
}
