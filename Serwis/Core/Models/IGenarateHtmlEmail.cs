using Serwis.Models.ViewModels;

namespace Serwis.Core.Models
{
    public interface IGenarateHtmlEmail
    {
        string GenerateInvoice(OrderViewModel order);
    }
}