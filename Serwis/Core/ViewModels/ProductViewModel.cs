using Serwis.Models.Domains;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serwis.Models.ViewModels
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {
            OrderPositions = new Collection<OrderPosition>();
            CategoriesList = new Collection<ProductCategory>(); // na potrzeby viewmodelu

        }
        [Display(Name = "Identyfikator")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Pole Nazwa musi zostac wypełnione")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Pole Wartość musi zostac wypełnione")]
        [Display(Name = "Wartość")]
        [Column(TypeName = "decimal(2,2)")]
        [Range(1, 100)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Pole Opis musi zostac wypełnione")]
        [Display(Name = "Opis")]
        public string Description { get; set; }

        public string? ImageFileName { get; set; }

        [Required(ErrorMessage = "Zdjęcie jest wymagane!")]
        [Display(Name = "Dodaj zdjęcie")]

        public int CategoryId { get; set; }
        
        public ProductCategory? Categories { get; set; }

        public ICollection<ProductCategory> CategoriesList { get; set; } 

        public IFormFile? ImageFile { get; set; }

        public DateTime CreatedDate { get; set; }
        
        public ICollection<OrderPosition> OrderPositions { get; set; }
    }
}