using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serwis.Models.Domains
{

    public class ApplicationUser
    {

        public ApplicationUser()
        {
            Orders = new Collection<Order>();
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Zapamietaj mnie")]
        public bool RememberMe { get; set; }
        public ICollection<Order> Orders { get; set; } //uztykownik moze miec wiele zamówień


        [EmailAddress]
        public string? Email { get; set; }
        public decimal? Wallet { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
