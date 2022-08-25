using System.ComponentModel.DataAnnotations;

namespace Serwis.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="Nazwa uzytkownika jest wymagana")]
        [Display(Name = "Nazwa uzytkownika")]
        public string UserName { get; set; }

        [Required(ErrorMessage ="Hasło jest wymagane")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Required(ErrorMessage ="Powtórzenie hasla jest wymagane")]
        [Display(Name = "Powtórz hasło")]
        public string RepeatPassword { get; set; }

        [Display(Name = "Zapamietaj mnie")]
        public bool RememberMe { get; set; }

        [Required(ErrorMessage = "Mail jest wymagany")]
        [EmailAddress]
        public string Email { get; set; }

        public decimal Wallet { get; set; }
        
        public DateTime CreatedDate { get; set; }
    }
}
