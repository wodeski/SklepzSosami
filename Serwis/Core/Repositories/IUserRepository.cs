using Serwis.Models.Domains;

namespace Serwis.Core.Repositories
{
    public interface IUserRepository
    {
        Task<bool> UserHasAnIncompleteOrder(Guid userId);

        Task<ApplicationUser> FindUserAsync(string userName);
        Task<ApplicationUser> FindUserByIdAsync(Guid userId);
    }
}
