using System.ComponentModel.DataAnnotations;

namespace Serwis.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Nazwa uzytkownika jest wymagana")]
        [Display(Name = "Nazwa uzytkownika")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Nazwa uzytkownika jest wymagana")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Required(ErrorMessage ="Nazwa uzytkownika jest wymagana")]
        [Display(Name = "Powtórz hasło")]
        public string RepeatPassword { get; set; }

        [Display(Name = "Zapamietaj mnie")]
        public bool RememberMe { get; set; }

        [Required(ErrorMessage = "Nazwa uzytkownika jest wymagana")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public decimal Wallet { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }
}
