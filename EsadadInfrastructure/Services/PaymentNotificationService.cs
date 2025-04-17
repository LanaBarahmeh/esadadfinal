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
                                                          && a.IsPaymentPosted);

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
                    CallUpdatePaymentPostedProcedure(guid);
                

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
        private void CallUpdatePaymentPostedProcedure(Guid guid)
        {
            using var conn = new OracleConnection("User Id=ESADAD; Password=esadad_password; Data Source=localhost:1521/XEPDB1");
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "usp_Update_IsPaymentPosted_ByGUID";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("p_guid", OracleDbType.Varchar2, 50).Value = guid.ToString();
            cmd.ExecuteNonQuery();
        }
    }
}