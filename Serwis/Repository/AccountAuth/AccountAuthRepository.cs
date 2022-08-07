using Serwis.Models;
using Serwis.Models.Domains;
using Microsoft.EntityFrameworkCore;


namespace Serwis.Repository.AccountAuth
{
    public class AccountAuthRepository
    {
        private readonly ServiceDbContext _serviceDbContext;
        public AccountAuthRepository(ServiceDbContext serviceDbContext)
        {
            _serviceDbContext = serviceDbContext;
        }

        public ApplicationUser FindUser(ApplicationUser credential)
        {
            var find = _serviceDbContext.Credentials
                .SingleOrDefault(x => x.UserName == credential.UserName && x.Password == credential.Password);
            if (find == null)
            {
                return null;
            }
            return find;
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
