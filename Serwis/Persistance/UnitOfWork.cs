using Serwis.Core;
using Serwis.Core.Models;
using Serwis.Core.Repositories;
using Serwis.Models;
using Serwis.Persistance.Repository;
using Serwis.Repository;

namespace Serwis.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IReportRepository _reportRepository;
        private readonly IGenarateHtmlEmail _genarateHtmlEmail;

        public UnitOfWork(IApplicationDbContext serviceDbContext)
        {
            _applicationDbContext = serviceDbContext;
            User = new UserRepository(serviceDbContext);
            ProductCategory = new CategoryRepository(serviceDbContext);
            Product = new ProductRepository(serviceDbContext);
            Order = new OrderRepository(serviceDbContext);
            OrderPosition = new OrderPositionRepository(serviceDbContext);
            AuthRepository = new AuthRepository(serviceDbContext);
            EmailSender = new EmailSender(_reportRepository);
            GenerateHtmlEmail = new GenarateHtmlEmail();
           // ReportRepository = new ReportRepository();
        }


        public IUserRepository User { get; set; }
        public ICategoryRepository ProductCategory { get; set; }
        public IProductRepository Product { get; set; }
        public IOrderRepository Order { get; set; }
        public IOrderPositionRepository OrderPosition { get; set; }
        public IAuthRepository AuthRepository { get; set; }
        public IEmailSender EmailSender { get; set; }
        public IReportRepository ReportRepository { get; set; }
        public IGenarateHtmlEmail GenerateHtmlEmail { get; set; }

        public void Complete()
        {
             _applicationDbContext.SaveChanges();
        }
       
    }
}
