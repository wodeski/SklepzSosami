using Serwis.Models.Domains;
using System.ServiceProcess;
using System;
using Serwis.Models.Extensions;

namespace Serwis.Models
{
    public class EmailSender
    {

        private ReportRepository _reportRepository;
        private readonly GenarateHtmlEmail genarateHtmlEmail;
        private Email _email;
        private GenarateHtmlEmail _genarateHtmlEmail;
        //private StringCipher _stringCipher = new StringCipher("6AD15A5C-2E1E-434A-9193-E5AF43E2D013");

            
        
        public EmailSender(ReportRepository reportRepository, GenarateHtmlEmail genarateHtmlEmail)
        {
            _reportRepository = reportRepository;
            _genarateHtmlEmail = genarateHtmlEmail;
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

        public async void SendMail(string emailReciever, IEnumerable<OrderPosition> orderPositionsFromCart) // przesyla email usera oraz liste produktów z koszyka
        {
            try
            {
                    await SendReport(emailReciever, orderPositionsFromCart);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }


        }
        private async Task SendReport(string emailReceiver, IEnumerable<OrderPosition> orderPositionsFromCart)
        {
            if (orderPositionsFromCart == null)
                return;
          var orderTitle = _reportRepository.ReportSent(orderPositionsFromCart);//zmiana statusu zamówienia
            
            await _email.Send("Informacje o zakupie", _genarateHtmlEmail.GenerateInvoice(orderPositionsFromCart, orderTitle), emailReceiver);

        }
    }
}
