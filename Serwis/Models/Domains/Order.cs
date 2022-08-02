using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serwis.Models.Domains
{
    public class Order
    {
        [Key]
        [Display(Name = "Identyfikator")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Pole Nazwa musi zostac wypełnione")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        public string? ImageFileName { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
