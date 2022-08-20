using Serwis.Models.ViewModels;

namespace Serwis.Models
{
    public interface IEmailSender
    {
        void SendMail(string emailReciever, OrderViewModel order);
    }
}