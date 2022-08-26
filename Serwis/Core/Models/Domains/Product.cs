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
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        [Range(1, 100)]
        public decimal Price { get; set; }

        [Required]
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