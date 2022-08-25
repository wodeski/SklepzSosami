using Serwis.Models.Domains;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Serwis.Models.ViewModels
{
    public class ApplicationUserViewModel 
    {
        public ApplicationUserViewModel()
        {
            Orders = new Collection<Order>();
        }

        public Guid Id { get; set; }

        [Required(ErrorMessage ="Pole login musi zostac wypełnione")]
        [Display(Name = "Nazwa uzytkownika")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Pole hasło musi zostac wypełnione")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Display(Name = "Zapamietaj mnie")]
        public bool RememberMe { get; set; }
        public ICollection<Order> Orders { get; set; } //uztykownik moze miec wiele zamówień

        public string? Email { get; set; }
        public decimal? Wallet { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}