using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serwis.Models.Domains
{
    public class Product
    {
        public Product()
        {
            OrderPositions = new Collection<OrderPosition>();
        }
        [Key]
        [Display(Name = "Identyfikator")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Pole Nazwa musi zostac wypełnione")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        public string? ImageFileName { get; set; }

        [Required(ErrorMessage = "Dodaj zdjęcie")]
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public ICollection<OrderPosition> OrderPositions { get; set; }
    }
}