using Esadad.Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Esadad.Infrastructure.Interfaces
{
    public interface IPrepaidValidationService
    {
        public PrePaidResponseDto GetInvalidSignatureResponse(Guid guid, string billingNumber, string serviceType, string prepaidCat, int validationCode);
        public PrePaidResponseDto GetResponse(Guid guid, XmlElement xmlElement);

    }
}
