using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serwis.Models.Domains
{

    public class ApplicationUser : IApplicationUser
    {

        public ApplicationUser()
        {
            Orders = new Collection<Order>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nazwa uzytkownika")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Display(Name = "Zapamietaj mnie")]
        public bool RememberMe { get; set; }
        public ICollection<Order> Orders { get; set; } //uztykownik moze miec wiele zamówień
        public string? Email { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
