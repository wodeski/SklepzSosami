using Serwis.Core.Service;
using Serwis.Models.Domains;
using Serwis.Repository;

namespace Serwis.Models
{
    public class ReportRepository
    {
        private readonly IService _service;
        public ReportRepository(IService service)
        {
            _service = service;   

        }
        public async Task<string> ReportSentAsync(IEnumerable<OrderPosition> orderPositions)
        {
            //pobieranie z BD
            var find = orderPositions.Select(x=>x.OrderId).First();
            var title = await _service.UpdateOrderAsync(find); // ustawienie zamówienia jako zrealizowanego
            return title;
        }
        
    }
}
