using Esadad.Infrastructure.Interfaces;
using Esadad.Infrastructure.DTOs;
using Esadad.Infrastructure.Helpers;
using Esadad.Infrastructure.MemCache;
using Esadad.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using Esadad.Infrastructure.Enums;

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
                //var existing = _context.TransactionsLogs
                //                       .FirstOrDefault(a => a.Guid.Equals(guid.ToString())
                //                                            && a.Type.Equals("Response")
                //                                            && a.IsPaymentPosted);

                //int procedureResult = 0;
                //if (existing != null)
                //{
                //    existing.Retry += 1;
                //    _context.SaveChanges();
                //}
                //else
                //{
                //    var panNoDesc = new SqlParameter("@panNoDesc", billingNumber);
                //    var paymentAmt = new SqlParameter("@paymentAmt", xmlElement.SelectSingleNode("//PaidAmt")?.InnerText);
                //    var convertedCurrency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).Currency.Equals("ILS") ? "NIS" : MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).Currency;

                //    var currncyCode = new SqlParameter("@ActCurCd", convertedCurrency);

                //    procedureResult = _context
                //                        .Database
                //                        .ExecuteSqlRaw("exec dbo.EsadadPayment @panNoDesc, @paymentAmt, @ActCurCd", panNoDesc, paymentAmt, currncyCode);
                //}

                //PaymentNotificationResponse response = new()
                //{
                //    MsgHeader = new MsgHeader()
                //    {
                //        TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                //        GUID = guid,
                //        TrsInf = new TrsInf
                //        {
                //            SdrCode = MemoryCache.Biller.Code,
                //            ResTyp = "BLRPMTNTFRS"
                //        },
                //        Result = new Result
                //        {
                //            ErrorCode = 0,
                //            ErrorDesc = "Success",
                //            Severity = "Info"
                //        }
                //    },
                //    MsgBody = new PaymentNotificationResponseBody()
                //    {
                //        Transactions = new PaymentNotificationResponseTransactions()
                //        {
                //            TrxInf = new PaymentNotificationResponseTrxInf()
                //            {
                //                JOEBPPSTrx = paymentNotificationRequestTrxInf.JOEBPPSTrx,
                //                ProcessDate = paymentNotificationRequestTrxInf.ProcessDate,
                //                STMTDate = paymentNotificationRequestTrxInf.STMTDate,
                //                Result = new Result()
                //                {
                //                    ErrorCode = 0,
                //                    ErrorDesc = "Success",
                //                    Severity = "Info"
                //                }
                //            }
                //        }
                //    }
                //};

                //var msgFooter = new MsgFooter()
                //{
                //    Security = new Security()
                //    {
                //        Signature = DigitalSignature.SignMessage(ObjectToXmlHelper.ObjectToXmlElement(response))
                //    }
                //};

                //response.MsgFooter = msgFooter;

                //if (existing == null)
                //{
                //    var logResult = _commonService.InsertLog(guid.ToString(), billingNumber, serviceType, ObjectToXmlHelper.ObjectToXmlElement(response), "Response");

                //    if (procedureResult > 0)
                //    {
                //        logResult.IsPaymentPosted = true;

                //        _context.SaveChanges();
                //    }
                //}

                return new PaymentNotificationResponseDto();
            }
            catch
            {
                throw;
            }
        }
    }
}