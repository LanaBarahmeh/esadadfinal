 using Esadad.Infrastructure.DTOs;
using Esadad.Infrastructure.Interfaces;
using Esadad.Infrastructure.Persistence;
using Esadad.Infrastructure.Helpers;
using Esadad.Infrastructure.MemCache;
using System.Xml;
using Esadad.Infrastructure.Enums;

namespace Esadad.Infrastructure.Services
{
    public class BillPullService(EsadadIntegrationDbContext context, ICommonService commonService) : IBillPullService
    {
        private readonly EsadadIntegrationDbContext _context = context;
        private readonly ICommonService _commonService = commonService;

        public BillPullResponse BillPull(Guid guid, XmlElement xmlElement)
        {
            BillPullRequest billPullRequestObj = null;
            try
            {
                BillPullRequest billPullRequest = new BillPullRequest();
                 billPullRequestObj= XmlToObjectHelper.DeserializeXmlToObject(xmlElement, billPullRequest);

                var curr = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == billPullRequestObj.MsgBody.ServiceType).Currency;

                // Query the internal system by billing number and other criteries as required

                var data =new {DueDate = DateTime.Now, DueAmount=20.0};
                if (data == null)
                {
                    // Billing Number is not found
                    return GetInvalidBillingNumberResponse(guid, billPullRequestObj.MsgBody.AcctInfo.BillingNo, billPullRequestObj.MsgBody.ServiceType);
                }
                else if (data.DueAmount > 0)  // Billing number id found and there is a bill with DueAmount
                {
                    return GetBillResponse(guid, billPullRequestObj.MsgBody.AcctInfo.BillingNo, billPullRequestObj.MsgBody.ServiceType, data.DueDate, (decimal)data.DueAmount);
                }
                else // Billing number id found and there is a bill without DueAmount
                {
                    return GetNoDueBillResponse(guid, billPullRequestObj.MsgBody.AcctInfo.BillingNo, billPullRequestObj.MsgBody.ServiceType, data.DueDate);
                }
            }
            catch (Exception e)
            {
                return GetGeneralExceptionResponse(guid, billPullRequestObj.MsgBody.AcctInfo.BillingNo, billPullRequestObj.MsgBody.ServiceType, e.Message);
            }
        }

        // Invalid Signature Response
        public BillPullResponse GetInvalidSignatureResponse(Guid guid, string billingNumber, string serviceType)
        {
            try
            {

                BillPullResponse response = new BillPullResponse()
                {
                    MsgHeader = new MsgHeader()
                    {
                        TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                        GUID = guid,
                        TrsInf = new TrsInf
                        {
                            SdrCode = MemoryCache.Biller.Code,
                            ResTyp = "BILPULRS"
                        },
                        Result = new Result
                        {
                            ErrorCode = 2,
                            ErrorDesc = "Invalid Signature",
                            Severity = "Error"
                        }
                    },
                    MsgBody = new BillPullResponseBody()
                    {
                        RecCount = 0
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

                // Log Response to EsadadTransactionLog

                var tranLog = _commonService.InsertLog(TransactionTypeEnum.Request.ToString(), ApiTypeEnum.BillPull.ToString(), guid.ToString(),null, response);

                return response;
            }
            catch
            {
                throw;
            }
        }

        // Bill Available with Due Amount 
        private BillPullResponse GetBillResponse(Guid guid, string billingNumber, string serviceType, DateTime DueDate, decimal dueAmount)
        {
            try
            {
                BillPullResponse response = new BillPullResponse()
                {
                    MsgHeader = new MsgHeader()
                    {
                        TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                        GUID = guid,
                        TrsInf = new TrsInf
                        {
                            SdrCode = MemoryCache.Biller.Code,
                            ResTyp = "BILPULRS"
                        },
                        Result = new Result
                        {
                            ErrorCode = 0,
                            ErrorDesc = "Success",
                            Severity = "Info"
                        }
                    },
                    MsgBody = new BillPullResponseBody()
                    {
                        RecCount = 1,
                        BillsRec = new BillsRec()
                        {
                            BillRec = new BillRec()
                            {
                                Result = new Result
                                {
                                    ErrorCode = 0,
                                    ErrorDesc = "Success",
                                    Severity = "Info"
                                },
                                AcctInfo = new AcctInfo()
                                {
                                    BillingNo = billingNumber,
                                    BillNo = billingNumber
                                },
                                BillStatus = "BillNew",
                                DueAmount = CurrencyHelper.AdjustDecimal(dueAmount, MemoryCache.Currencies[MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).Currency], DecimalAdjustment.Truncate),
                                Currency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).Currency,
                                IssueDate = DateTime.Parse(DueDate.ToString("yyyy-MM-ddTHH:mm:ss")),
                                DueDate = DateTime.Parse(DueDate.ToString("yyyy-MM-ddTHH:mm:ss")),
                                ServiceType = serviceType,
                                PmtConst = new PmtConst()
                                {
                                    AllowPart = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).PartialPayment, // allow partial payment if yes them can paid partially
                                    Lower = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).PartialPayment? MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).MinimumAmount: CurrencyHelper.AdjustDecimal(dueAmount, MemoryCache.Currencies[MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).Currency], DecimalAdjustment.Truncate), // اقل قيمه للدفع
                                    Upper = CurrencyHelper.AdjustDecimal(dueAmount, MemoryCache.Currencies[MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).Currency], DecimalAdjustment.Truncate),  // اكثر قيمه للدفع
                                },
                                SubPmts = new SubPmts()
                                {
                                    SubPmt = new SubPmt()
                                    {
                                        Amount = CurrencyHelper.AdjustDecimal(dueAmount, MemoryCache.Currencies[MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).Currency],DecimalAdjustment.Truncate),
                                        SetBnkCode = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).BankCode,
                                        AcctNo = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).IBAN
                                    }
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

                // Log Response to EsadadTransactionLog

                var tranLog = _commonService.InsertLog(TransactionTypeEnum.Request.ToString(), ApiTypeEnum.BillPull.ToString(), guid.ToString(), null, response);

                return response;
            }
            catch
            {
                throw;
            }
        }

        //Bill with No Due Amount Response
        private BillPullResponse GetNoDueBillResponse(Guid guid, string billingNumber, string serviceType, DateTime DueDate)
        {
            try
            {
                BillPullResponse response = new BillPullResponse()
                {
                    MsgHeader = new MsgHeader()
                    {
                        TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                        GUID = guid,
                        TrsInf = new TrsInf
                        {
                            SdrCode = MemoryCache.Biller.Code,
                            ResTyp = "BILPULRS"
                        },
                        Result = new Result
                        {
                            ErrorCode = 0,
                            ErrorDesc = "Success",
                            Severity = "Info"
                        }
                    },
                    MsgBody = new BillPullResponseBody()
                    {
                        RecCount = 1,
                        BillsRec = new BillsRec()
                        {
                            BillRec = new BillRec()
                            {
                                Result = new Result
                                {
                                    ErrorCode = 0,
                                    ErrorDesc = "Success",
                                    Severity = "Info"
                                },
                                AcctInfo = new AcctInfo()
                                {
                                    BillingNo = billingNumber,
                                    BillNo = billingNumber
                                },
                                BillStatus = "BillNew",
                                DueAmount = 0,
                                Currency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).Currency,
                                IssueDate = DateTime.Parse(DueDate.ToString("yyyy-MM-ddTHH:mm:ss")),
                                DueDate = DateTime.Parse(DueDate.ToString("yyyy-MM-ddTHH:mm:ss")),
                                ServiceType = serviceType,
                                PmtConst = new PmtConst()
                                {
                                    AllowPart = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).PartialPayment,
                                    Lower = 0,
                                    Upper = 0
                                },
                                SubPmts = new SubPmts()
                                {
                                    SubPmt = new SubPmt()
                                    {
                                        Amount = 0,
                                        SetBnkCode = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).BankCode,
                                        AcctNo = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).IBAN
                                    }
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

                // Log Response to EsadadTransactionLog

                var tranLog = _commonService.InsertLog(TransactionTypeEnum.Request.ToString(), ApiTypeEnum.BillPull.ToString(), guid.ToString(), null, response);

                return response;
            }
            catch
            {
                throw;
            }
        }

        //Invalid Billing Number Response
        private BillPullResponse GetInvalidBillingNumberResponse(Guid guid, string billingNumber, string serviceType)
        {
            try
            {

                BillPullResponse response = new BillPullResponse()
                {
                    MsgHeader = new MsgHeader()
                    {
                        TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                        GUID = guid,
                        TrsInf = new TrsInf
                        {
                            SdrCode = MemoryCache.Biller.Code,
                            ResTyp = "BILPULRS"
                        },
                        Result = new Result
                        {
                            ErrorCode = 408,
                            ErrorDesc = "Invalid Billing Number",
                            Severity = "Error"
                        }
                    },
                    MsgBody = new BillPullResponseBody()
                    {
                        RecCount = 0
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

                // Log Response to EsadadTransactionLog

                var tranLog = _commonService.InsertLog(TransactionTypeEnum.Request.ToString(), ApiTypeEnum.BillPull.ToString(), guid.ToString(), null, response);

                return response;
            }
            catch
            {
                throw;
            }
        }

        //Generel Exception Response
        private BillPullResponse GetGeneralExceptionResponse(Guid guid, string billingNumber, string serviceType, string exceptionMessage)
        {
            try
            {
                BillPullResponse response = new BillPullResponse()
                {
                    MsgHeader = new MsgHeader()
                    {
                        TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                        GUID = guid,
                        TrsInf = new TrsInf
                        {
                            SdrCode = MemoryCache.Biller.Code,
                            ResTyp = "BILPULRS"
                        },
                        Result = new Result
                        {
                            ErrorCode = 303,
                            ErrorDesc = exceptionMessage,
                            Severity = "Error"
                        }
                    },
                    MsgBody = new BillPullResponseBody()
                    {
                        RecCount = 0
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

                // Log Response to EsadadTransactionLog

                var tranLog = _commonService.InsertLog(TransactionTypeEnum.Request.ToString(), ApiTypeEnum.BillPull.ToString(), guid.ToString(), null, response);

                return response;
            }
            catch
            {
                throw;
            }
        }

       
    }
}