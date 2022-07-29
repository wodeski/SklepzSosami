using System.ComponentModel.DataAnnotations;

namespace Serwis.Models.Domains
{
    public class Order
    {
        [Key]
        [Display(Name="Identyfikator")]
        [Required(ErrorMessage ="Pole Id musi zostac wypełnione")]
        public int Id { get; set; }

        [Required(ErrorMessage="Pole Nazwa musi zostac wypełnione")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }
    }
}
