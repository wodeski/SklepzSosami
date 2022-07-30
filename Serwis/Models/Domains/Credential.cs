using System.ComponentModel.DataAnnotations;

namespace Serwis.Models.Domains
{
    public class Credential
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Nazwa uzytkownika")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Display(Name = "Zapamietaj mnie")]
        public bool RememberMe { get; set; }
    }
}
