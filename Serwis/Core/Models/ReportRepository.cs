using Serwis.Core.Service;
using Serwis.Models.Domains;
using Serwis.Repository;

namespace Serwis.Models
{
    public class ReportRepository : IReportRepository
    {
        private readonly IService _service;
        public ReportRepository(IService service)
        {
            _service = service;

        }
        public async Task ReportSentAsync(IEnumerable<OrderPosition> orderPositions)
        {
            //pobieranie z BD
            var find = orderPositions.Select(x => x.OrderId).First();
        }

    }
}
