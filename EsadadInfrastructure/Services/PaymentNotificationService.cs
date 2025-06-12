using Esadad.Infrastructure.Interfaces;
using Esadad.Infrastructure.DTOs;
using Esadad.Infrastructure.Helpers;
using Esadad.Infrastructure.MemCache;
using Esadad.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using Esadad.Infrastructure.Enums;
using Oracle.ManagedDataAccess.Client;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data;
using Esadad.Core.Entities;
using log4net;
using System.Xml.Linq;

namespace Esadad.Infrastructure.Services
{
    public class PaymentNotificationService(EsadadIntegrationDbContext context, ICommonService commonService) : IPaymentNotificationService
    {
        private readonly EsadadIntegrationDbContext _context = context;
        private readonly ICommonService _commonService = commonService;

        public PaymentNotificationResponseDto GetInvalidSignatureResponse(Guid guid, string billingNumber, string serviceType, XmlElement xmlElement)
        {
            try
            {

                PaymentNotificationResponseDto response = new PaymentNotificationResponseDto()
                {
                    MsgHeader = new MsgHeader()
                    {
                        TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                        GUID = guid,
                        TrsInf = new TrsInf
                        {
                            SdrCode = MemoryCache.Biller.Code,
                            ResTyp = "BLRPMTNTFRS"
                        },
                        Result = new Result
                        {
                            ErrorCode = 2,
                            ErrorDesc = "Invalid Signature",
                            Severity = "Error"
                        }
                    },
                    MsgBody = new PaymentNotificationResponseBody() { }
                };

                var msgFooter = new MsgFooter()
                {
                    Security = new Security()
                    {
                        Signature = DigitalSignature.SignMessage(ObjectToXmlHelper.ObjectToXmlElement(response))
                    }
                };

                response.MsgFooter = msgFooter;

                //Log to EsadadTransactionsLogs Table
                var tranLog = _commonService.InsertLog(TransactionTypeEnum.Response.ToString(), ApiTypeEnum.ReceivePaymentNotification.ToString(), guid.ToString(), xmlElement);

                return response;
            }
            catch
            {
                throw;
            }
        }

        public PaymentNotificationResponseDto GetPaymentNotificationResponse(Guid guid,
                                                                          string billingNumber,
                                                                          string serviceType,
                                                                          PaymentNotificationResponseTrxInf paymentNotificationRequestTrxInf,
                                                                          XmlElement xmlElement)
        {
            try
            {
                //check table esadad payment by guid and [IsPaymentPosted] true
                var existing = _context.EsadadPaymentsLogs
                                       .FirstOrDefault(a => a.Guid.Equals(guid.ToString())
                                                          && a.IsPaymentPosted == true);

                PaymentNotificationResponseDto response ; 
                int procedureResult = 0;
                if (existing != null)
                {
                    // existing.Retry += 1;
                    //  _context.SaveChanges();

                    //fill response
                    response = new()
                    {
                        MsgHeader = new MsgHeader()
                        {
                            TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                            GUID = guid,
                            TrsInf = new TrsInf
                            {
                                SdrCode = MemoryCache.Biller.Code,
                                ResTyp = "BLRPMTNTFRS"
                            },
                            Result = new Result
                            {
                                ErrorCode = 0,
                                ErrorDesc = "Success",
                                Severity = "Info"
                            }
                        },
                        MsgBody = new PaymentNotificationResponseBody()
                        {
                            Transactions = new PaymentNotificationResponseTransactions()
                            {
                                TrxInf = new PaymentNotificationResponseTrxInf()
                                {
                                    JOEBPPSTrx = paymentNotificationRequestTrxInf.JOEBPPSTrx,
                                    ProcessDate = paymentNotificationRequestTrxInf.ProcessDate,
                                    STMTDate = paymentNotificationRequestTrxInf.STMTDate,
                                    Result = new Result()
                                    {
                                        ErrorCode = 0,
                                        ErrorDesc = "Success",
                                        Severity = "Info"
                                    }
                                }
                            }
                        }
                    };

                    var msgFooter1 = new MsgFooter()
                    {
                        Security = new Security()
                        {
                            Signature = DigitalSignature.SignMessage(ObjectToXmlHelper.ObjectToXmlElement(response))
                        }
                    };

                    response.MsgFooter = msgFooter1;

                }
                else
                {
                    //both have to be transactions
                    // create stored procedure to reflect the payment to our local system and update esadad paynmet transaction log is payment posted true (maximum id where guid= ...)
                    //update [EsadadPaymentsLogs] set [IsPaymentPosted] = true where [Guid] = existing GUID and id = (select MAX(id ) from EsadadPaymentsLogs where [Guid] = existing guid ordery by id desc )
                    CallUpdatePaymentPostedAndInsertProcedure(xmlElement, paymentNotificationRequestTrxInf, guid);
                

                    //add .netcore to execute sp

                    response = new()
                    {
                        MsgHeader = new MsgHeader()
                        {
                            TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                            GUID = guid,
                            TrsInf = new TrsInf
                            {
                                SdrCode = MemoryCache.Biller.Code,
                                ResTyp = "BLRPMTNTFRS"
                            },
                            Result = new Result
                            {
                                ErrorCode = 0,
                                ErrorDesc = "Success",
                                Severity = "Info"
                            }
                        },
                        MsgBody = new PaymentNotificationResponseBody()
                        {
                            Transactions = new PaymentNotificationResponseTransactions()
                            {
                                TrxInf = new PaymentNotificationResponseTrxInf()
                                {
                                    JOEBPPSTrx = paymentNotificationRequestTrxInf.JOEBPPSTrx,
                                    ProcessDate = paymentNotificationRequestTrxInf.ProcessDate,
                                    STMTDate = paymentNotificationRequestTrxInf.STMTDate,
                                    Result = new Result()
                                    {
                                        ErrorCode = 0,
                                        ErrorDesc = "Success",
                                        Severity = "Info"
                                    }
                                }
                            }
                        }
                    };

                    var msgFooter = new MsgFooter()
                    {
                        Security = new Security()
                        {
                            Signature = DigitalSignature.SignMessage(ObjectToXmlHelper.ObjectToXmlElement(response))
                        }
                    };

                    response.MsgFooter = msgFooter;

                }
                if (response != null)
                {
                    var logResult = _commonService.InsertLog(TransactionTypeEnum.Response.ToString(), ApiTypeEnum.ReceivePaymentNotification.ToString(), guid.ToString(),  ObjectToXmlHelper.ObjectToXmlElement(response));

                    //if (procedureResult > 0)
                    //{
                    //    logResult.IsPaymentPosted = true;

                    //    _context.SaveChanges();
                    //}
                }

                return response;
            }
            catch
            {
                throw;
            }
        }
        private void CallUpdatePaymentPostedAndInsertProcedure(XmlElement xmlElement, PaymentNotificationResponseTrxInf trx, Guid guid)
        {
            var request = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new PaymentNotificationRequestDto());
            var trxInfo = request.MsgBody.Transactions.TrxInf;
            var subPmt = trxInfo.SubPmts.SubPmt;
            var serviceDetails = trxInfo.ServiceTypeDetails;
            var currency = MemoryCache.Biller.Services
                .FirstOrDefault(b => b.ServiceTypeCode == serviceDetails.ServiceType)?.Currency ;

            using var conn = new OracleConnection("User Id=LANA; Password=yslm@2024; Data Source=172.16.5.56:1521/Xroad");
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "usp_UpdatePostedMoiPayments";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_guid", OracleDbType.Varchar2, 50).Value = guid.ToString();
            cmd.Parameters.Add("p_bill_no", OracleDbType.Varchar2).Value = trxInfo.AcctInfo.BillNo;
            cmd.Parameters.Add("p_billing_no", OracleDbType.Varchar2).Value = trxInfo.AcctInfo.BillingNo;
            cmd.Parameters.Add("p_joebppstrx", OracleDbType.Varchar2).Value = trx.JOEBPPSTrx;
            cmd.Parameters.Add("p_bank_trx_id", OracleDbType.Varchar2).Value = trxInfo.BankTrxID.ToString();
            cmd.Parameters.Add("p_bank_code", OracleDbType.Varchar2).Value = trxInfo.BankCode;
            cmd.Parameters.Add("p_pmt_status", OracleDbType.Varchar2).Value = trxInfo.PmtStatus;

            cmd.Parameters.Add("p_due_amt", OracleDbType.Decimal).Value = trxInfo.DueAmt;
            cmd.Parameters.Add("p_paid_amt", OracleDbType.Decimal).Value = trxInfo.PaidAmt;
            cmd.Parameters.Add("p_fees_amt", OracleDbType.Decimal).Value = trxInfo.FeesAmt;
            cmd.Parameters.Add("p_fees_on_biller", OracleDbType.Varchar2).Value = trxInfo.FeesOnBiller;

            cmd.Parameters.Add("p_process_date", OracleDbType.Date).Value = trxInfo.ProcessDate;
            cmd.Parameters.Add("p_stmt_date", OracleDbType.Date).Value = trxInfo.STMTDate;

            cmd.Parameters.Add("p_access_channel", OracleDbType.Varchar2).Value = trxInfo.AccessChannel;
            cmd.Parameters.Add("p_payment_method", OracleDbType.Varchar2).Value = trxInfo.PaymentMethod;
            cmd.Parameters.Add("p_payment_type", OracleDbType.Varchar2).Value = trxInfo.PaymentType;

            cmd.Parameters.Add("p_currency", OracleDbType.Varchar2).Value = currency;
            cmd.Parameters.Add("p_service_type", OracleDbType.Varchar2).Value = serviceDetails.ServiceType;
            cmd.Parameters.Add("p_prepaid_cat", OracleDbType.Varchar2).Value = serviceDetails.PrepaidCat;

            cmd.Parameters.Add("p_sub_amt", OracleDbType.Decimal).Value = subPmt.Amount;
            cmd.Parameters.Add("p_sub_set_bnk_code", OracleDbType.Varchar2).Value = subPmt.SetBnkCode;
            cmd.Parameters.Add("p_sub_acct_no", OracleDbType.Varchar2).Value = subPmt.AcctNo;

            cmd.ExecuteNonQuery();
        }





    }
}