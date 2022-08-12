using Serwis.Core;
using Serwis.Models.Domains;

namespace Serwis.Repository
{
    public class AuthRepository
    {
        private readonly IApplicationDbContext _applicationDbContext;
        public AuthRepository(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public ApplicationUser FindUserWithLoginCredentials(string userName, string password)
        {
            var find = _applicationDbContext.Credentials
                .SingleOrDefault(x => x.UserName == userName && x.Password == password);
            if (find == null)
            {
                return null;
            }
            return find;
        }

        public bool IsUserNameFromRegisterValid(string userName)
        {
            var find = _applicationDbContext.Credentials
                .SingleOrDefault(x => x.UserName == userName);
            if (find == null)
            {
                return true;
            }
            return false;
        }

        public ApplicationUser GetUser(string userName)
        {
            var find = _applicationDbContext.Credentials
                .SingleOrDefault(x => x.UserName == userName);
            if (find == null)
            {
                throw new ArgumentNullException();
            }
            return find;
        }

        public void CreateUser(ApplicationUser user)
        {
            _applicationDbContext.Credentials.Add(user);
            _applicationDbContext.SaveChanges();
        }
    }
}
