using Esadad.Infrastructure.DTOs;
using System.Xml;

namespace Esadad.Infrastructure.Interfaces
{
    public interface IPaymentNotificationService
    {
        public PaymentNotificationResponseDto GetPaymentNotificationResponse(Guid guid, string billingNumber, string serviceType, PaymentNotificationResponseTrxInf paymentNotificationRequestTrxInf, XmlElement xmlElement);

        public PaymentNotificationResponseDto GetInvalidSignatureResponse(Guid guid, string billingNumber, string serviceType, XmlElement xmlElement);
    }
}