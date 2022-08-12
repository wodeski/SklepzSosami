using Serwis.Core;
using Serwis.Core.Repositories;
using Serwis.Persistance.Repository;

namespace Serwis.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IApplicationDbContext _applicationDbContext;

        public UnitOfWork(IApplicationDbContext serviceDbContext)
        {
            _applicationDbContext = serviceDbContext;
            User = new UserRepository(serviceDbContext);
            ProductCategory = new CategoryRepository(serviceDbContext);
            Product = new ProductRepository(serviceDbContext);
            Order = new OrderRepository(serviceDbContext);
            OrderPosition = new OrderPositionRepository(serviceDbContext);
        }


        public IUserRepository User { get; set; }
        public ICategoryRepository ProductCategory { get; set; }
        public IProductRepository Product { get; set; }
        public IOrderRepository Order { get; set; }
        public IOrderPositionRepository OrderPosition { get; set; }

        public void Complete()
        {
             _applicationDbContext.SaveChanges();
        }
       
    }
}
