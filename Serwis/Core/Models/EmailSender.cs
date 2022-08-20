using Serwis.Models.Domains;
using System.ServiceProcess;
using System;
using Serwis.Models.Extensions;
using Serwis.Core.Models;
using Serwis.Models.ViewModels;
using Serwis.Core.Service;
using Serwis.Persistance;

namespace Serwis.Models
{
    public class EmailSender : IEmailSender
    {
        private readonly IService _service;
        private readonly IReportRepository _reportRepository;
        private Email _email;
        //private StringCipher _str ingCipher = new StringCipher("6AD15A5C-2E1E-434A-9193-E5AF43E2D013");



        public EmailSender(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
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

        

        //private string DecryptSenderEmailPassword()
        //{
        //    var encryptedPassword = "fjwowpepjgtnkjcf"; 
        //    return _stringCipher.Decrypt(encryptedPassword);
        //}

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

            await _email.Send("Informacje o zamówieniu",body , emailReceiver);

        }
    }
}
