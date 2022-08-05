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

        public bool FindUser(ApplicationUser credential)
        {
            try
            {
                var find = _serviceDbContext.Credentials
                    .SingleOrDefault(x => x.UserName == credential.UserName && x.Password == credential.Password);
                if (find !=null)
                {
                    return true;
                }
            }
            catch
            {
                throw new ArgumentException();
            }
            return false;
    }
    }
}
