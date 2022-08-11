using Serwis.Models.Domains;
using Microsoft.EntityFrameworkCore;
using Serwis.Models.Service;

namespace Serwis.Repository.AccountAuth
{
    public class AccountAuthRepository
    {
        private readonly ServiceDbContext _serviceDbContext;
        public AccountAuthRepository(ServiceDbContext serviceDbContext)
        {
            _serviceDbContext = serviceDbContext;
        }

        public ApplicationUser FindUserWithLoginCredentials(string userName, string password)
        {
            var find = _serviceDbContext.Credentials
                .SingleOrDefault(x => x.UserName == userName && x.Password == password);
            if (find == null)
            {
                return null;
            }
            return find;
        }

        public bool IsUserNameFromRegisterValid(string userName)
        {
            var find = _serviceDbContext.Credentials
                .SingleOrDefault(x => x.UserName ==userName);
            if (find == null)
            {
                return true;
            }
            return false;
        }

        public ApplicationUser GetUser(string userName)
        {
            var find = _serviceDbContext.Credentials
                .SingleOrDefault(x => x.UserName == userName);
            if (find == null)
            {
                throw new ArgumentNullException();
            }
            return find;
        }

        public void CreateUser(ApplicationUser user)
        {
            _serviceDbContext.Credentials.Add(user);
            _serviceDbContext.SaveChanges();
        }
    }
}
