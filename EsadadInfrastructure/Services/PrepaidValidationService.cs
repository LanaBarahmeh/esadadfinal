using Esadad.Infrastructure.DTOs;
using Esadad.Infrastructure.Enums;
using Esadad.Infrastructure.Helpers;
using Esadad.Infrastructure.Interfaces;
using Esadad.Infrastructure.MemCache;
using Esadad.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Esadad.Infrastructure.Services
{
    public class PrepaidValidationService(EsadadIntegrationDbContext context, ICommonService commonService) : IPrepaidValidationService
    {
        private readonly EsadadIntegrationDbContext _context = context;
        private readonly ICommonService _commonService = commonService;
        public PrePaidResponseDto GetInvalidSignatureResponse(Guid guid, string billingNumber, string serviceType, string prepaidCat, int validationCode)
        {
            try
            {

                PrePaidResponseDto response = new PrePaidResponseDto()
                {
                    MsgHeader = new MsgHeader()
                    {
                        TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                        GUID = guid,
                        TrsInf = new TrsInf
                        {
                            SdrCode = MemoryCache.Biller.Code,
                            ResTyp = "BILRPREPADVALRS"
                        },
                        Result = new Result
                        {
                            ErrorCode = 0,
                            ErrorDesc = "Success",
                            Severity = "Info"
                        }
                    },
                    MsgBody = new PrePaidResponseBody()
                    {
                        BillingInfo = new BillingInfo()
                        {
                             Result = new Result()
                             {
                                 ErrorCode = 2,
                                 ErrorDesc = "Invalid Signature",
                                 Severity = "Error"
                             },
                             AcctInfo = new PrepaidAcctInfo()
                             {
                                 BillingNo = billingNumber,
                                 BillerCode = MemoryCache.Biller.Code
                             },
                             DueAmt=0,
                             Currency= MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).Currency,
                             ValidationCode = validationCode,
                             ServiceTypeDetails = new ServiceTypeDetails()
                             {
                                  ServiceType= serviceType
                             },
                             SubPmts = new SubPmts()
                             {
                                 SubPmt = new SubPmt()
                                 {
                                     Amount = CurrencyHelper.AdjustDecimal(0, MemoryCache.Currencies[MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).Currency], DecimalAdjustment.Truncate),
                                     SetBnkCode = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).BankCode,
                                     AcctNo = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).IBAN
                                 }
                             }
                        }
                    }
                };

                if(prepaidCat != null || prepaidCat != "")
                {
                    response.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat = prepaidCat;
                }

             

                var msgFooter = new MsgFooter()
                {
                    Security = new Security()
                    {
                        Signature = DigitalSignature.SignMessage(ObjectToXmlHelper.ObjectToXmlElement(response))
                    }
                };
                response.MsgFooter = msgFooter;

                // Log Response to EsadadTransactionLog

                var tranLog = _commonService.InsertLog(TransactionTypeEnum.Response.ToString(), ApiTypeEnum.PrepaidValidation.ToString(), guid.ToString(), ObjectToXmlHelper.ObjectToXmlElement(response), response);

                return response;
            }
            catch
            {
                throw;
            }
        }


        public PrePaidResponseDto GetResponse(Guid guid, XmlElement xmlElement)
        {
            try
            {

                var prepaidValidationRequestObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new PrePaidRequestDto());


                PrePaidResponseDto response = new PrePaidResponseDto()
                {
                    MsgHeader = new MsgHeader()
                    {
                        TmStp = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")),
                        GUID = guid,
                        TrsInf = new TrsInf
                        {
                            SdrCode = MemoryCache.Biller.Code,
                            ResTyp = "BILRPREPADVALRS"
                        },
                        Result = new Result
                        {
                            ErrorCode = 0,
                            ErrorDesc = "Success",
                            Severity = "Info"
                        }
                    },
                    MsgBody = new PrePaidResponseBody()
                    {
                        BillingInfo = new BillingInfo()
                        {
                            Result = new Result()
                            {
                                ErrorCode = 0,
                                ErrorDesc = "Success",
                                Severity = "Info"
                            },
                            AcctInfo = new PrepaidAcctInfo()
                            {
                                BillingNo = prepaidValidationRequestObj.MsgBody.BillingInfo.AcctInfo.BillingNo,
                                BillerCode = MemoryCache.Biller.Code
                            },
                            DueAmt = 0,
                            Currency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType).Currency,
                            ValidationCode = prepaidValidationRequestObj.MsgBody.BillingInfo.ValidationCode,
                            ServiceTypeDetails = new ServiceTypeDetails()
                            {
                                ServiceType = prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType
                            },
                            SubPmts = new SubPmts()
                            {
                                SubPmt = new SubPmt()
                                {

                                    // rertrive service type category value (Replace 0)
                                    Amount = CurrencyHelper.AdjustDecimal(0, MemoryCache.Currencies[MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType).Currency], DecimalAdjustment.Truncate),
                                    SetBnkCode = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType).BankCode,
                                    AcctNo = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType).IBAN
                                }
                            }
                        }
                    }
                };

                if (prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat != null )
                {
                    response.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat = prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat;
                }



                var msgFooter = new MsgFooter()
                {
                    Security = new Security()
                    {
                        Signature = DigitalSignature.SignMessage(ObjectToXmlHelper.ObjectToXmlElement(response))
                    }
                };
                response.MsgFooter = msgFooter;

                // Log Response to EsadadTransactionLog

                var tranLog = _commonService.InsertLog(TransactionTypeEnum.Response.ToString(), ApiTypeEnum.PrepaidValidation.ToString(), guid.ToString(), ObjectToXmlHelper.ObjectToXmlElement(response), response);

                return response;
            }
            catch
            {
                throw;
            }

        }

    }
}
