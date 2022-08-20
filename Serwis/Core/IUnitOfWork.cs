using Serwis.Core.Models;
using Serwis.Core.Repositories;
using Serwis.Models;

namespace Serwis.Core
{
    public interface IUnitOfWork
    {
        IOrderRepository Order { get; set; }
        IOrderPositionRepository OrderPosition { get; set; }
        IProductRepository Product { get; set; }
        ICategoryRepository ProductCategory { get; set; }
        IUserRepository User { get; set; }
        IAuthRepository AuthRepository { get; set; }
        IEmailSender EmailSender { get; set; }
        IReportRepository ReportRepository { get; set; }
        IGenarateHtmlEmail GenerateHtmlEmail{ get; set; }

        void Complete();
    }
}