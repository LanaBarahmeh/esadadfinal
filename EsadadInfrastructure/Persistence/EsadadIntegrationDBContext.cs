using Esadad.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Esadad.Infrastructure.Persistence
{
    public class EsadadIntegrationDbContext : DbContext
    {
        public DbSet<EsadadTransactionLog> EsadadTransactionsLogs { get; set; }
        public DbSet<EsadadPaymentLog> EsadadPaymentsLogs { get; set; }


        public EsadadIntegrationDbContext(DbContextOptions<EsadadIntegrationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           

            modelBuilder.Entity<EsadadTransactionLog>(entity =>
            {
                entity.ToTable("ESADADTRANSACTIONSLOGS"); 
                entity.HasKey(e => e.Id);

                entity.Property(e => e.TransactionType)
                      .IsRequired()
                      .HasColumnType("VARCHAR2(50)")
                      .HasColumnName("TRANSACTIONTYPE");

                entity.Property(e => e.ApiName)
                      .IsRequired()
                      .HasColumnType("NVARCHAR2(255)")
                       .HasColumnName("APINAME"); 

                entity.Property(e => e.Guid)
                      .IsRequired()
                      .HasColumnType("VARCHAR2(50)")
                       .HasColumnName("GUID");

                entity.Property(e => e.Timestamp)
                      .IsRequired()
                      .HasColumnType("DATE")
                       .HasColumnName("TIMESTAMP");


                entity.Property(e => e.BillingNumber)
                    .HasColumnName("BILLINGNUMBER")
                    .HasColumnType("NVARCHAR2(50)");

                entity.Property(e => e.BillNumber)
                    .HasColumnName("BILLNUMBER")
                    .HasColumnType("NVARCHAR2(50)");

                entity.Property(e => e.Currency)
                    .HasColumnName("CURRENCY")
                    .HasColumnType("VARCHAR2(10)");

                entity.Property(e => e.ServiceType)
                    .HasColumnName("SERVICETYPE")
                    .HasColumnType("NVARCHAR2(50)");

                entity.Property(e => e.PrepaidCat)
                    .HasColumnName("PREPAIDCAT")
                    .HasColumnType("NVARCHAR2(50)");

                entity.Property(e => e.ValidationCode)
                    .HasColumnName("VALIDATIONCODE")
                    .HasColumnType("VARCHAR2(50)");

                entity.Property(e => e.TranXmlElement)
                      .IsRequired()
                      .HasColumnType("CLOB")
                       .HasColumnName("TRANXMLELEMENT");

                entity.Property(e => e.InsertDate)
                      .HasColumnType("DATE")
                      .HasColumnName("INSERTDATE");
            });
            modelBuilder.Entity<EsadadPaymentLog>(entity =>
            {
                entity.ToTable("ESADADPAYMENTLOGS");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Guid)
                    .IsRequired()
                    .HasColumnName("GUID")
                    .HasMaxLength(50);

                entity.Property(e => e.BillingNumber)
                    .IsRequired()
                    .HasColumnName("BILLINGNUMBER")
                    .HasMaxLength(50);

                entity.Property(e => e.BillNumber)
                    .IsRequired()
                    .HasColumnName("BILLNUMBER")
                    .HasMaxLength(50);

                entity.Property(e => e.PaidAmount)
                    .IsRequired()
                    .HasColumnName("PAIDAMOUNT")
                    .HasColumnType("decimal(12, 3)");

                entity.Property(e => e.JOEBPPSTrx)
                    .IsRequired()
                    .HasColumnName("JOEBPPSTRX")
                    .HasMaxLength(50);

                entity.Property(e => e.BankTrxID)
                    .IsRequired()
                    .HasColumnName("BANKTRXID")
                    .HasMaxLength(50);

                entity.Property(e => e.BankCode)
                    .IsRequired()
                    .HasColumnName("BANKCODE");

                entity.Property(e => e.DueAmt)
                    .IsRequired()
                    .HasColumnName("DUEAMOUNT")
                    .HasColumnType("decimal(12, 3)");

               

                entity.Property(e => e.FeesAmt)
                    .HasColumnName("FEESAMOUNT")
                    .HasColumnType("decimal(12, 3)");

                entity.Property(e => e.FeesOnBiller)
                    .IsRequired()
                    .HasColumnName("FEESONBILLER")
                    .HasColumnType("bit")
                    .HasDefaultValue(false);

                entity.Property(e => e.ProcessDate)
                    .IsRequired()
                    .HasColumnName("PROCESSDATE");

                entity.Property(e => e.STMTDate)
                    .IsRequired()
                    .HasColumnName("STMTDATE");

                entity.Property(e => e.AccessChannel)
                    .IsRequired()
                    .HasColumnName("ACCESSCHANNEL")
                    .HasMaxLength(50);

                entity.Property(e => e.PaymentMethod)
                    .IsRequired()
                    .HasColumnName("PAYMENTMETHOD")
                    .HasMaxLength(50);

                entity.Property(e => e.PaymentType)
                    .IsRequired()
                    .HasColumnName("PAYMENTTYPE")
                    .HasMaxLength(50);

                entity.Property(e => e.Currency)
                    .IsRequired()
                    .HasColumnName("CURRENCY")
                    .HasMaxLength(10);

                entity.Property(e => e.ServiceType)
                    .IsRequired()
                    .HasColumnName("SERVICETYPE")
                    .HasMaxLength(50);

                entity.Property(e => e.PrepaidCat)
                    .HasColumnName("PREPAIDCATEGORY")
                    .HasMaxLength(50);

                entity.Property(e => e.Amount)
                    .IsRequired()
                    .HasColumnName("AMOUNT")
                    .HasColumnType("decimal(12, 3)");

                entity.Property(e => e.SetBnkCode)
                    .IsRequired()
                    .HasColumnName("SETBNKCODE");

                entity.Property(e => e.AcctNo)
                    .IsRequired()
                    .HasColumnName("ACCTNO")
                    .HasMaxLength(50);

                entity.Property(e => e.IsPaymentPosted)
                    .IsRequired()
                    .HasColumnName("ISPAYMENTPOSTED")
                    .HasColumnType("bit")
                    .HasDefaultValue(false);

                entity.Property(e => e.InsertDate)
                    .IsRequired()
                    .HasColumnName("INSERTDATE")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("GETDATE()");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

