using Microsoft.EntityFrameworkCore;
using Serwis.Core;
using Serwis.Core.Repositories;
using Serwis.Models.Domains;

namespace Serwis.Persistance.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IApplicationDbContext _serviceDbContext;
        public UserRepository(IApplicationDbContext serviceDbContext)
        {
            _serviceDbContext = serviceDbContext;
        }
        public async Task<ApplicationUser> FindUserAsync(string userName)
        {
            //poprawic warunki
            var findUser = await _serviceDbContext.Credentials.Where(o => o.UserName == userName).FirstOrDefaultAsync();
            if (findUser == null)
            {
                return null;
            }
            return findUser;
        }
        public async Task<ApplicationUser> FindUserByIdAsync(Guid userId)
        {
            var findUser = await _serviceDbContext.Credentials.Where(x => x.Id == userId).FirstAsync();
            if (findUser == null)
            {
                return null;
            }
            return findUser;
        }

        public async Task<bool> UserHasAnIncompleteOrder(Guid userId)
        {
            return await _serviceDbContext.Orders.AnyAsync(x => x.UserId == userId && x.IsCompleted == false); ;
        }

      
    }
}
