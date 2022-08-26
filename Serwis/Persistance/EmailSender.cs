using Serwis.Models.Domains;
using System.ServiceProcess;
using System;
using Serwis.Models.Extensions;
using Serwis.Core.Models;
using Serwis.Models.ViewModels;
using Serwis.Core.Service;
using Serwis.Models;

namespace Serwis.Persistance
{
    public class EmailSender : IEmailSender
    {
        private Email _email;
        public EmailSender()
        {
            try
            {
                _email = new Email(new EmailParams
                {
                    HostSmtp = AppSettingsGetter.AppSetting["EmailCredential:HostSmtp"],
                    EnableSsl = bool.Parse(AppSettingsGetter.AppSetting["EmailCredential:EnableSsl"]),
                    Port = int.Parse(AppSettingsGetter.AppSetting["EmailCredential:Port"]),
                    SenderName = AppSettingsGetter.AppSetting["EmailCredential:SenderName"],
                    SenderEmail = AppSettingsGetter.AppSetting["EmailCredential:SenderEmail"],
                    SenderEmailPassword = AppSettingsGetter.AppSetting["EmailCredential:Password"]
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async void SendMail(string emailReciever, OrderViewModel order) // przesyla email usera oraz liste produktów z koszyka
        {
            //ToString mozna skrócic
            try
            {
                await SendReportAsync(emailReciever, order);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        private async Task SendReportAsync(string emailReceiver, OrderViewModel order)
        {
            // List<OrderPosition> orderPosition = new List<OrderPosition>();//zeby bledu nie wywalilo 
            //  await _reportRepository.ReportSentAsync(orderPosition);//zmiana statusu zamówienia
            GenarateHtmlEmail genarateHtmlEmail = new();

            var body = genarateHtmlEmail.GenerateInvoice(order);

            await _email.Send("Informacje o zamówieniu", body, emailReceiver);

        }
    }
}
