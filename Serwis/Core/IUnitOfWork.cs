using Serwis.Core.Repositories;

namespace Serwis.Core
{
    public interface IUnitOfWork
    {
        IOrderRepository Order { get; set; }
        IOrderPositionRepository OrderPosition { get; set; }
        IProductRepository Product { get; set; }
        ICategoryRepository ProductCategory { get; set; }
        IUserRepository User { get; set; }

        void Complete();
    }
}