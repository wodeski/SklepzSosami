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
        public DbSet<ApplicationUser> Credentials { get; set; }
        public DbSet<OrderPosition> OrderPositions { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(x => x.Orders)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
                .HasMany(x => x.OrderPositions)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Product>()
                .HasMany(x => x.OrderPositions)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Product>()
                .Property(x => x.Price)
                .HasColumnType("decimal(5,2)");

            modelBuilder.Entity<ApplicationUser>()
               .Property(x => x.Wallet)
               .HasColumnType("decimal(5,2)");

            base.OnModelCreating(modelBuilder);
           
                
               
        }
    }
}

