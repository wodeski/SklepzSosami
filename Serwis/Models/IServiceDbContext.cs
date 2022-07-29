using Microsoft.EntityFrameworkCore;
using Serwis.Models.Domains;

namespace Serwis.Models
{
    public interface IServiceDbContext
    {
        DbSet<Order> Orders { get; set; }
    }
}