using Microsoft.EntityFrameworkCore;
using Serwis.Models.Domains;

namespace Serwis.Models.Service
{
    public interface IServiceDbContext
    {
        DbSet<Order> Orders { get; set; }
        DbSet<ApplicationUser> Credentials { get; set; }
        DbSet<OrderPosition> OrderPositions { get; set; }
        DbSet<Product> Products { get; set; }
    }
}