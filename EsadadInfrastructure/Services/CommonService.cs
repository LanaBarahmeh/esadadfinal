using Esadad.Core.Entities;
using Esadad.Infrastructure.DTOs;
using Esadad.Infrastructure.Helpers;
using Esadad.Infrastructure.Interfaces;
using Esadad.Infrastructure.MemCache;
using Esadad.Infrastructure.Persistence;
using System.Xml;
using log4net;
using log4net.Config;
using System.Reflection;

namespace Esadad.Infrastructure.Services
{

    public class CommonService(EsadadIntegrationDbContext context) : ICommonService
    {


        private static readonly ILog log = LogManager.GetLogger("Task");
        private readonly EsadadIntegrationDbContext _context = context;
        EsadadPaymentLog ICommonService.InsertPaymentLog(string transactionType, string apiName, string guid, XmlElement requestElement)
        {
            throw new NotImplementedException();
        }
    public EsadadTransactionLog InsertLog(string transactionType, string apiName, string guid, XmlElement xmlElement, Object responseObject = null)
        {
            try
            {
                //BillPullRequest billPullRequestObj = null;
                EsadadTransactionLog esadadTransactionLog = null;

                

                // Reference the log4net.config from Project2 output folder
                //// Set log4net global context property (e.g., machine name)
                //GlobalContext.Properties["host"] = Environment.MachineName;

                //// Reference the log4net.config from Project2 output folder
                //var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());

                //// Ensure the log4net.config file is available in the output directory (e.g., bin/Debug/netcoreapp3.1)
                //var log4netConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");

                //if (File.Exists(log4netConfigPath))
                //{
                    
                //    XmlConfigurator.Configure(logRepository, new FileInfo(log4netConfigPath));
                   
                //}
                //else
                //{
                //    Console.WriteLine("log4net.config file not found at path: " + log4netConfigPath);
                //}


                //FileInfo fi = new FileInfo("log4net.config");
                //log4net.Config.XmlConfigurator.Configure(fi);
                //log4net.GlobalContext.Properties["host"] = Environment.MachineName;

                log.Info("hello");
                //log.Info($"InsertLog started | Type: {transactionType}, API: {apiName}, Guid: {guid}");


                //PaymentNotificationRequestDto paymentNotificationRequestDtoObj = null;

                if (transactionType.ToLower() == "request")
                {
                    if (apiName == "BillPull")
                    {
                        log.Info("Deserializing BillPull request...");
                        var billPullRequestObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new BillPullRequest());
                        esadadTransactionLog = new EsadadTransactionLog
                        {
                            TransactionType = transactionType,
                            ApiName = apiName,
                            Guid = guid,
                            Timestamp = billPullRequestObj.MsgHeader.TmStp,
                            BillingNumber = billPullRequestObj.MsgBody.AcctInfo.BillingNo,
                            BillNumber = billPullRequestObj.MsgBody.AcctInfo.BillNo,
                            ServiceType = billPullRequestObj.MsgBody.ServiceType,
                            Currency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == billPullRequestObj.MsgBody.ServiceType).Currency,
                            TranXmlElement = xmlElement.OuterXml
                        };

                    }
                    else if (apiName == "ReceivePaymentNotification")
                    {
                        log.Info("Deserializing ReceivePaymentNotification request...");
                        var paymentNotificationRequestDtoObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new PaymentNotificationRequestDto());
                        esadadTransactionLog = new EsadadTransactionLog
                        {
                            TransactionType = transactionType,
                            ApiName = apiName,
                            Guid = guid,
                            Timestamp = paymentNotificationRequestDtoObj.MsgHeader.TmStp,
                            BillingNumber = paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.AcctInfo.BillingNo,
                            BillNumber = paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.AcctInfo.BillNo,
                            ServiceType = paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.ServiceTypeDetails.ServiceType,
                            Currency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.ServiceTypeDetails.ServiceType).Currency,
                            ValidationCode = paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.AcctInfo.BillNo,
                            PrepaidCat = paymentNotificationRequestDtoObj.MsgBody.Transactions.TrxInf.ServiceTypeDetails.PrepaidCat,
                            TranXmlElement = xmlElement.OuterXml
                        };
                    }
                    else if (apiName == "PrepaidValidation")
                    {
                        log.Info("Deserializing PrepaidValidation request...");
                        var prepaidValidationRequestObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new PrePaidRequestDto());
                        esadadTransactionLog = new EsadadTransactionLog
                        {
                            TransactionType = transactionType,
                            ApiName = apiName,
                            Guid = guid,
                            Timestamp = prepaidValidationRequestObj.MsgHeader.TmStp,
                            BillingNumber = prepaidValidationRequestObj.MsgBody.BillingInfo.AcctInfo.BillingNo,
                            ServiceType = prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType,
                            PrepaidCat = prepaidValidationRequestObj.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat,
                            TranXmlElement = xmlElement.OuterXml
                        };

                    }

                }
                else if (transactionType.ToLower() == "response" && xmlElement != null)
                {
                    log.Info("Processing response...");
                    if (apiName == "BillPull")
                    {
                        var billPullResponseObj = (BillPullResponse)responseObject;
                        esadadTransactionLog = new EsadadTransactionLog
                        {
                            TransactionType = transactionType,
                            ApiName = apiName,
                            Guid = guid,
                            Timestamp = billPullResponseObj.MsgHeader.TmStp,
                            BillingNumber = billPullResponseObj.MsgBody.BillsRec.BillRec.AcctInfo.BillingNo,
                            BillNumber = billPullResponseObj.MsgBody.BillsRec.BillRec.AcctInfo.BillNo,
                            ServiceType = billPullResponseObj.MsgBody.BillsRec.BillRec.ServiceType,
                            Currency = MemoryCache.Biller.Services.First(b => b.ServiceTypeCode == billPullResponseObj.MsgBody.BillsRec.BillRec.ServiceType).Currency,
                            TranXmlElement = xmlElement.OuterXml
                        };

                    }
                    else if (apiName == "ReceivePaymentNotification")
                    {
                        var paymentNotificationResponseDtoObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new PaymentNotificationResponseDto());
                        esadadTransactionLog = new EsadadTransactionLog
                        {
                            TransactionType = transactionType,
                            ApiName = apiName,
                            Guid = guid,
                            Timestamp = paymentNotificationResponseDtoObj.MsgHeader.TmStp,
                            TranXmlElement = xmlElement.OuterXml
                        };

                    }
                    else if (apiName == "PrepaidValidation")
                    {
                        var prepaidValidationResponseObj = XmlToObjectHelper.DeserializeXmlToObject(xmlElement, new PrePaidResponseDto());
                        esadadTransactionLog = new EsadadTransactionLog
                        {
                            TransactionType = transactionType,
                            ApiName = apiName,
                            Guid = guid,
                            Timestamp = prepaidValidationResponseObj.MsgHeader.TmStp,
                            BillingNumber = prepaidValidationResponseObj.MsgBody.BillingInfo.AcctInfo.BillingNo,
                            ServiceType = prepaidValidationResponseObj.MsgBody.BillingInfo.ServiceTypeDetails.ServiceType,
                            PrepaidCat = prepaidValidationResponseObj.MsgBody.BillingInfo.ServiceTypeDetails.PrepaidCat,
                            TranXmlElement = xmlElement.OuterXml
                        };

                    }


                }
                log.Info("Saving log to database...");
                var query = _context.EsadadTransactionsLogs.Add(esadadTransactionLog).Entity;

                _context.SaveChanges();

                log.Info("Log saved successfully.");
                return query;
            }
            catch (Exception ex)
            {
                log.Error("Error in InsertLog", ex);
                throw;
            }

            //EsadadTransactionLog ICommonService.InsertLog(string transactionType, string apiName, string guid, XmlElement requestElement, object responseObject)
            //{
            //    throw new NotImplementedException();
            //}

              

        }
    }
}