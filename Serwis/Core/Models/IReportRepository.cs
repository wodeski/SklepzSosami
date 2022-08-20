using Serwis.Models.Domains;

namespace Serwis.Models
{
    public interface IReportRepository
    {
        Task ReportSentAsync(IEnumerable<OrderPosition> orderPositions);
    }
}