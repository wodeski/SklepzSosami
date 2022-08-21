using Serwis.Models.Domains;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Serwis.Models.ViewModels
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            OrderPositions = new Collection<OrderPosition>();
        }
        public int Id { get; set; } //jak zwiekszac inta zrobic view model
       
        [Display(Name ="Nazwa zamowienia")]
        public string? Title { get; set; }

        public bool IsCompleted { get; set; }

        [Display(Name = "Łączna wartość zamówienia")]
        public decimal FullPrice { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } // klucz obcy
        public ICollection<OrderPosition> OrderPositions { get; set; }
    }
}