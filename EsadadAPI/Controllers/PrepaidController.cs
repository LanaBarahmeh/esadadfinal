using Esadad.Infrastructure.DTOs;
using Esadad.Infrastructure.Enums;
using Esadad.Infrastructure.Helpers;
using Esadad.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml;


namespace EsadadAPI.Controllers
{
    [Route("api/[controller]")]
    [Consumes("application/xml")]
    [Produces("application/xml")]
    [ApiController]
    public class PrepaidController : ControllerBase
    {
        // private readonly IBillPullService _billPullService;
        //IBillPullService billPullService,
        private readonly ICommonService _commonService;
        private readonly IPrepaidValidationService _prepaidValidationService;
        public PrepaidController(IPrepaidValidationService prepaidValidationService, ICommonService commonService)
        {
            _prepaidValidationService = prepaidValidationService;
            _commonService = commonService;
        }

        [HttpPost("PrepaidValidation")]
        public IActionResult PrepaidValidation([FromQuery(Name = "GUID")] Guid guid,
                                     [FromBody] XmlElement xmlElement,
                                     [FromQuery(Name = "username")] string? username = null,
                                     [FromQuery(Name = "password")] string? password = null)
        {


            // Log Request 
            string? billingNumber = xmlElement.SelectSingleNode("//BillingNo")?.InnerText;
            string? serviceType = xmlElement.SelectSingleNode("//ServiceType")?.InnerText;
            string? prepaidCat = xmlElement.SelectSingleNode("//PrepaidCat")?.InnerText;
            int validatioCode = int.Parse(xmlElement.SelectSingleNode("//ValidationCode")?.InnerText);

            //Log to EsadadTransactionsLogs Table
            var tranLog = _commonService.InsertLog(TransactionTypeEnum.Request.ToString(), ApiTypeEnum.PrepaidValidation.ToString(), guid.ToString(), xmlElement);

            PrePaidResponseDto prePaidResponseDto = null;
            if (!DigitalSignature.VerifySignature(xmlElement))
            {

                prePaidResponseDto = _prepaidValidationService.GetInvalidSignatureResponse(guid, billingNumber, serviceType, prepaidCat, validatioCode);

                return Ok(prePaidResponseDto);
            }
            else
            {
                //Log Response
                prePaidResponseDto = _prepaidValidationService.GetResponse(guid, xmlElement);

                return Ok(prePaidResponseDto);
            }


            return Ok(new PrePaidResponseDto());

        }

    }
}
