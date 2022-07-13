using System.ComponentModel.DataAnnotations;

namespace Serwis.Models.Domains
{
    public class ServiceOrder
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Client { get; set; }

        [Required]
        public bool Guarantee { get; set; }

        [Required]
        public string Description { get; set; }
        public string? SerialNumber { get; set; }
        public int AmountOfCycles { get; set; }
        public int YearOfProduction { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
