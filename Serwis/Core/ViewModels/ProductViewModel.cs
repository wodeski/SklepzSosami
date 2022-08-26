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
        [MaxLength(50, ErrorMessage ="Nazwa jest za długa!")]
        public string Name { get; set; }

        [RegularExpression(@"^\d+\.\d{2}$")]
        [Required(ErrorMessage = "Pole Wartość musi zostac wypełnione")]
        [Display(Name = "Wartość")]
        [Range(1,100, ErrorMessage =  "Pole musi miescic sie w przedziale od 1 do 100")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Pole Opis musi zostac wypełnione")]
        [Display(Name = "Opis")]
        [MaxLength(250,ErrorMessage ="Podanny opis jest za długi!")]
        public string Description { get; set; }

        public string? ImageFileName { get; set; }


        [Required(ErrorMessage ="Wybierz kategorię!")]
        [Display(Name = "Kategoria")]
        public int CategoryId { get; set; }
        
        public ProductCategory? Categories { get; set; }

        public IEnumerable<ProductCategory> CategoriesList { get; set; } //bylo icollection

        [Display(Name="Plik zdjęciowy")]
        public IFormFile? ImageFile { get; set; }

        public DateTime CreatedDate { get; set; }
        
        public ICollection<OrderPosition> OrderPositions { get; set; }
    }
}