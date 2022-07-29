using Microsoft.EntityFrameworkCore;
using Serwis.Models.Domains;

namespace Serwis.Models
{
    public class ServiceDbContext : DbContext, IServiceDbContext
    {
        public ServiceDbContext(DbContextOptions<ServiceDbContext> options) : base(options)
        {

        }
        public DbSet<Order> Orders { get; set; }
    }
}

    