using Serwis.Models.Domains;
using System.ServiceProcess;
using System;
using Serwis.Models.Extensions;

namespace Serwis.Models
{
    public class EmailSender
    {
        private ReportRepository _reportRepository = new ReportRepository();
        private Email _email;
        private GenarateHtmlEmail _htmlEmail = new GenarateHtmlEmail();
        //private StringCipher _stringCipher = new StringCipher("6AD15A5C-2E1E-434A-9193-E5AF43E2D013");

            
        
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
        //private string DecryptSenderEmailPassword()
        //{
        //    var encryptedPassword = "fjwowpepjgtnkjcf"; 
        //    return _stringCipher.Decrypt(encryptedPassword);
        //}

        public async void SendMail(string emailReciever)
        {
            try
            {
                await SendReport(emailReciever);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }


        }
        private async Task SendReport(string emailReceiver)
        {
            var report = _reportRepository.GetLastNotSendRepository();
            if (report == null)
                return;
            _reportRepository.ReportSent(report);
            await _email.Send("Informacje o zakupie", _htmlEmail.GenerateInvoice(report), emailReceiver);

        }
    }
}
