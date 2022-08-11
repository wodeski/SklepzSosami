using Serwis.Models.Domains;
using Serwis.Repository;

namespace Serwis.Models
{
    public class ReportRepository
    {
        private readonly IRepository _repository;
        public ReportRepository(IRepository repository)
        {
            _repository = repository;   

        }
        public async Task<string> ReportSentAsync(IEnumerable<OrderPosition> orderPositions)
        {
            //pobieranie z BD
            var find = orderPositions.Select(x=>x.OrderId).First();
            var title = await _repository.UpdateOrderAsync(find); // ustawienie zamówienia jako zrealizowanego
            return title;
        }
        
    }
}
