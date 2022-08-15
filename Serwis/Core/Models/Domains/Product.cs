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

        [Required(ErrorMessage = "Pole Wartość musi zostac wypełnione")]
        [Display(Name = "Wartość")]
        [Column(TypeName = "decimal(5,2)")]
        [Range(1, 100)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Pole Opis musi zostac wypełnione")]
        [Display(Name = "Opis")]
        [MaxLength(250)]
        public string Description { get; set; }

        public string? ImageFileName { get; set; }

        [NotMapped]
        [Display(Name ="Dodaj zdjęcie")]
        public IFormFile? ImageFile { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("Categories")]
        public int CategoryId { get; set; }
        public ProductCategory Categories { get; set; }
        public ICollection<OrderPosition> OrderPositions { get; set; }
    }
}