using Microsoft.EntityFrameworkCore;
using Serwis.Controllers;
using Serwis.Core;
using Serwis.Core.Repositories;
using Serwis.Models.Domains;
using Serwis.ShopControllers;

namespace Serwis.Persistance.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IApplicationDbContext _serviceDbContext;
        private ApplicationUser _admin;
        private ApplicationUser _anonymous;
        public UserRepository(IApplicationDbContext serviceDbContext)
        {
            _serviceDbContext = serviceDbContext;

            _admin = new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Email = "",
                Password = "admin",
                UserName = "admin"
            };
            _anonymous = new()
            {
                Id = Guid.NewGuid(),
                CreatedDate = DateTime.Now,
                Email = "",
                Password = "",
                UserName = "anonim"
            };
        }
        //jesli nie ma zadnych produktów w bazie danych doaj je z repozytorium
        //jesli nie ma admina i anonima dodaj ich
        //jesli probujesz logowac sie jako anonim zablokuj te mozliwosc

        public async Task CreateAdmin()
        {
            await _serviceDbContext.Credentials.AddAsync(_admin);
        }

        public async Task CreateAnonymous()
        {
            await _serviceDbContext.Credentials.AddAsync(_anonymous);
        }


        public async Task<bool> IsAnonymousCreated()
        {
            var findAnonymous = await _serviceDbContext.Credentials.SingleOrDefaultAsync(x => x.UserName == AccountController.IsAnonymous);

            if (findAnonymous == null)
            {
                return false;
            }
            return true;

        }
        public async Task<bool> IsAdminCreated()
        {
            var findAdmin = await _serviceDbContext.Credentials.SingleOrDefaultAsync(x => x.UserName == AccountController.IsAdmin);
            if (findAdmin == null)
            {
                return false;
            }
            return true;
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
