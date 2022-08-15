using Serwis.Models.Domains;
using System.Collections.ObjectModel;

namespace Serwis.Models.ViewModels
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            OrderPositions = new Collection<OrderPosition>();
        }
        public int Id { get; set; } //jak zwiekszac inta zrobic view model
        public string? Title { get; set; }

        public bool IsCompleted { get; set; }
        public decimal FullPrice { get; set; }
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } // klucz obcy
        public ICollection<OrderPosition> OrderPositions { get; set; }
    }
}