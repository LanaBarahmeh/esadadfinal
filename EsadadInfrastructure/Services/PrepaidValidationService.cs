using Esadad.Infrastructure.DTOs;
using Esadad.Infrastructure.Enums;
using Esadad.Infrastructure.Helpers;
using Esadad.Infrastructure.Interfaces;
using Esadad.Infrastructure.MemCache;
using Esadad.Infrastructure.Persistence;
using log4net.Core;
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
        public PrePaidResponseDto GetInvalidSignatureResponse(Guid guid, string billingNumber, string serviceType, string prepaidCat, int validationCode )
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
        public PrePaidResponseDto GetInvalidbillingResponse(Guid guid, string billingNumber, string serviceType, string prepaidCat, int validationCode, int errorcode, string errordesc)
        {
            decimal dueAmt = prepaidCat switch
            {
                "50_ILS" => 60,
                "100_ILS" => 110,
                
            };

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
                                ErrorCode = errorcode,
                                ErrorDesc = errordesc,
                                Severity = "Error"
                            },
                            AcctInfo = new PrepaidAcctInfo()
                            {
                                BillingNo = billingNumber,
                                BillerCode = MemoryCache.Biller.Code
                            },
                            DueAmt = dueAmt,
                            Currency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).Currency,
                            ValidationCode = validationCode,
                            ServiceTypeDetails = new ServiceTypeDetails()
                            {
                                ServiceType = serviceType
                            },
                            SubPmts = new SubPmts()
                            {
                                SubPmt = new SubPmt()
                                {
                                    Amount = CurrencyHelper.AdjustDecimal(dueAmt, MemoryCache.Currencies[MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).Currency], DecimalAdjustment.Truncate),
                                    SetBnkCode = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).BankCode,
                                    AcctNo = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceType).IBAN
                                }
                            }
                        }
                    }
                };

                if (prepaidCat != null || prepaidCat != "")
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
                var billingNo = prepaidValidationRequestObj.MsgBody?.BillingInfo?.AcctInfo?.BillingNo;

            
               

                var serviceTypeCode = prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType;
                var service = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == serviceTypeCode);
                var currency = MemoryCache.Currencies[service.Currency];
                var prepaidCat = prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat;
                var validationCode = prepaidValidationRequestObj.MsgBody.BillingInfo.ValidationCode;
                var errorcode = 2;
                var errordesc = "success";
                if (string.IsNullOrEmpty(billingNo) || !IsIdEquValid(billingNo))
                {
                     errorcode = 408;
                     errordesc = "Invalid Billing Number";
                    return GetInvalidbillingResponse(guid, billingNo, serviceTypeCode, prepaidCat, validationCode, errorcode, errordesc);
                }
                decimal dueAmt = prepaidCat switch
                {
                    "50_ILS" => 50,
                    "100_ILS" => 100
                };

                var response = new PrePaidResponseDto()
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
                                BillingNo = billingNo,
                                BillerCode = MemoryCache.Biller.Code
                            },
                            DueAmt = dueAmt,
                            Currency = service.Currency,
                            ValidationCode = prepaidValidationRequestObj.MsgBody.BillingInfo.ValidationCode,
                            ServiceTypeDetails = new ServiceTypeDetails()
                            {
                                ServiceType = serviceTypeCode,
                                PrepaidCat = prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat,
                                
                            },
                            SubPmts = new SubPmts()
                            {
                                SubPmt = new SubPmt()
                                {
                                    Amount = CurrencyHelper.AdjustDecimal(dueAmt, currency, DecimalAdjustment.Truncate),
                                    SetBnkCode = service.BankCode,
                                    AcctNo = service.IBAN
                                }
                            },
                            AdditionalInfo = new AdditionalInfo()
                            {
                                CustName = "لانا جمال محمد براهمة",
                                FreeText = "عزيزي المستخدم، يوجد مبلغ مستحق، يرجى المتابعة لإتمام عملية الدفع، شكراً لاستخدامك خدمة إي سداد, للاستفسار يرجى التواصل على الرقم"
                            }
                        }
                    }
                };

                response.MsgFooter = new MsgFooter()
                {
                    Security = new Security()
                    {
                        Signature = DigitalSignature.SignMessage(ObjectToXmlHelper.ObjectToXmlElement(response))
                    }
                };

                var tranLog = _commonService.InsertLog(
                    TransactionTypeEnum.Response.ToString(),
                    ApiTypeEnum.PrepaidValidation.ToString(),
                    guid.ToString(),
                    ObjectToXmlHelper.ObjectToXmlElement(response),
                    response
                );

                return response;
            }
            catch
            {
                throw;
            }
        }


        private static bool IsIdEquValid(string strId)
        {
            bool isValid = false;
            try
            {


                int _id;
                if (int.TryParse(strId, out _id))
                {
                    if (_id > 0 && _id.ToString().Length == 9)
                    {
                        int lastNo;

                        int i = 0;
                        int IDSum = 0;
                        string IDV = strId;
                        int temp;

                        while (i <= 7)
                        {
                            temp = int.Parse(IDV.Substring(i, 1));
                            if (int.Parse(IDV.Substring(i, 1)) != 9)
                            {
                                int a = int.Parse(IDV.Substring(i, 1));
                                int b = (i + 1 - 1) % 2;

                                IDSum = IDSum + (a * (b + 1)) % 9;
                            }
                            else
                            {
                                IDSum = IDSum + 9;
                            }

                            i += 1;
                        }


                        lastNo = (10 - (IDSum % 10)) % 10;

                        if (lastNo == int.Parse(strId.Substring(8, 1)))
                        {
                            isValid = true;
                        }
                    }
                }

            }
            catch (Exception)
            {


            }

            return isValid;
        }


    }
}
