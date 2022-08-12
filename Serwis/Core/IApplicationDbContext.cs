using Microsoft.EntityFrameworkCore;
using Serwis.Models.Domains;

namespace Serwis.Core
{
    public interface IApplicationDbContext
    {
        DbSet<Order> Orders { get; set; }
        DbSet<ApplicationUser> Credentials { get; set; }
        DbSet<OrderPosition> OrderPositions { get; set; }
        DbSet<Product> Products { get; set; }
        DbSet<ProductCategory> ProductCategories { get; set; }

        int SaveChanges();
    }
}