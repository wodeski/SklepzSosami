using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serwis.Models.Domains
{
    public class Order
    {

        public Order()
        {
            OrderPositions = new Collection<OrderPosition>();
        }
        [Key]
        [Display(Name = "Identyfikator")]
        public int Id { get; set; } //jak zwiekszac inta zrobic view model
        public string? Title { get; set; }

        public bool IsCompleted { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } // klucz obcy
        public ICollection<OrderPosition> OrderPositions { get; set; } //zamówienie moze miec wiele pozycji
    }
}
