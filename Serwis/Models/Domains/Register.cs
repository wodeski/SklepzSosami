using System.ComponentModel.DataAnnotations;

namespace Serwis.Models.Domains
{
    public class Register : IApplicationUser
    {
        [Required]
        [Display(Name = "Nazwa uzytkownika")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Pole email nie moze pozostać puste")]
        [Display(Name = "Podaj Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Powtórz hasło")]
        public string RepeatPassword { get; set; }

        public DateTime CreatedDate { get; set; }
           }
}
