using Serwis.Models.Domains;

namespace Serwis.Core.Repositories
{
    public interface IAuthRepository
    {
        Task CreateUser(ApplicationUser user);
        Task<ApplicationUser> FindUserWithLoginCredentials(string userName, string password);
        Task<ApplicationUser> GetUser(string userName);
        Task<bool> IsUserNameFromRegisterValid(string userName);
    }
}