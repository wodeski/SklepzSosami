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
        public string ReportSent(IEnumerable<OrderPosition> orderPositions)
        {
            //pobieranie z BD
            var find = orderPositions.Select(x=>x.OrderId).First();
            var title = _repository.UpdateOrder(find); // ustawienie zamówienia jako zrealizowanego
            return title;
        }
        
    }
}
