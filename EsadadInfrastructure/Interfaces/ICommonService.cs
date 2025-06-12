using Esadad.Core.Entities;
using Esadad.Infrastructure.DTOs;
using System.Xml;

namespace Esadad.Infrastructure.Interfaces
{
    public interface ICommonService
    {
        EsadadTransactionLog InsertLog(string transactionType, string apiName, string guid,
                                XmlElement requestElement, Object responseObject=null);
        EsadadPaymentLog InsertPaymentLog(XmlElement xmlElement, Guid guid);

    }
}
