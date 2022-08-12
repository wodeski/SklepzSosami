﻿using Serwis.Models.Domains;

namespace Serwis.Core.Repositories
{
    public interface IUserRepository
    {
        Task<bool> UserHasAnIncompleteOrder(int userId);

        Task<ApplicationUser> FindUserAsync(string userName);
        Task<ApplicationUser> FindUserByIdAsync(int userId);
    }
}