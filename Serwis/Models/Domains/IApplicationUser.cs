namespace Serwis.Models.Domains
{
    public interface IApplicationUser
    {
        string UserName { get; set; }
        string Password { get; set; }
        string Email { get; set; }
        DateTime CreatedDate { get; set; }
    }
}
