using Esadad.Infrastructure.DTOs;
using Esadad.Infrastructure.Enums;
using Esadad.Infrastructure.Helpers;
using Esadad.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace EsadadAPI.Controllers
{
    [Route("payment")]
    [ApiController]
    [Consumes("application/xml")]
    [Produces("application/xml")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentNotificationService _paymentNotificationService;
        private readonly ICommonService _commonService;

        public PaymentController(IPaymentNotificationService paymentNotificationService, ICommonService commonService)
        {
            _paymentNotificationService = paymentNotificationService;
            _commonService = commonService;
        }

        [HttpPost("ReceivePaymentNotification")]
        public IActionResult ReceivePaymentNotification([FromQuery(Name = "GUID")] Guid guid,
                                                        [FromBody] XmlElement xmlElement,
                                                        [FromQuery(Name = "username")] string? username = null,
                                                        [FromQuery(Name = "password")] string? password = null)
        {
            string? billingNumber = xmlElement.SelectSingleNode("//BillingNo")?.InnerText;
            string? serviceType = xmlElement.SelectSingleNode("//ServiceType")?.InnerText;

            //Log to EsadadTransactionsLogs Table


            var tranLog = _commonService.InsertLog(TransactionTypeEnum.Request.ToString(), ApiTypeEnum.ReceivePaymentNotification.ToString(), guid.ToString(), xmlElement);


            //Log to EsadadOaymentsLogs Table

            PaymentNotificationResponseDto paymentNotificationResponse;

            if (!DigitalSignature.VerifySignature(xmlElement))
            {
                paymentNotificationResponse = _paymentNotificationService.GetInvalidSignatureResponse(guid, billingNumber, serviceType, xmlElement);
                return Ok(paymentNotificationResponse);
            }

            var requestTrxInfo = new PaymentNotificationResponseTrxInf()
            {
                JOEBPPSTrx = xmlElement.SelectSingleNode("//JOEBPPSTrx")?.InnerText,
                ProcessDate = DateTime.Parse(xmlElement.SelectSingleNode("//ProcessDate")?.InnerText),
                STMTDate = xmlElement.SelectSingleNode("//STMTDate")?.InnerText
            };

            paymentNotificationResponse = _paymentNotificationService.GetPaymentNotificationResponse(guid, billingNumber, serviceType, requestTrxInfo, xmlElement);
            return Ok(paymentNotificationResponse);
        }
    }
}