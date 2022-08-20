using Microsoft.EntityFrameworkCore;
using Serwis.Core;
using Serwis.Core.Repositories;
using Serwis.Models.Domains;

namespace Serwis.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IApplicationDbContext _applicationDbContext;
        public AuthRepository(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<ApplicationUser> FindUserWithLoginCredentials(string userName, string password)
        {
            var find = await _applicationDbContext.Credentials
                .SingleOrDefaultAsync(x => x.UserName == userName && x.Password == password);
            if (find == null)
            {
                return null;
            }
            return find;
        }

        public async Task<bool> IsUserNameFromRegisterValid(string userName)
        {
            var find = await _applicationDbContext.Credentials
                .SingleOrDefaultAsync(x => x.UserName == userName);
            if (find == null)
            {
                return true;
            }
            return false;
        }

        public async Task<ApplicationUser> GetUser(string userName)
        {
            var find = await _applicationDbContext.Credentials
                .SingleOrDefaultAsync(x => x.UserName == userName);
            if (find == null)
            {
                throw new ArgumentNullException();
            }
            return find;
        }

        public async Task CreateUser(ApplicationUser user)
        {
            await _applicationDbContext.Credentials.AddAsync(user);
        }
    }
}
