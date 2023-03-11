using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace EBookShopR.Areas.Identity.Services
{
    public class EmailSender:IEmailSender
    {

        public async Task SendEmailAsync(string email,string subject,string htmlmessage)
        {
            using (var Client = new SmtpClient())
            {
                var Credential = new NetworkCredential
                {
                    UserName = "ershadrostami116",
                    Password = "gfounalsketbraga",
                };

                Client.Credentials = Credential;
                Client.Host ="smtp.gmail.com";
                Client.Port = 587;
                Client.EnableSsl = true;

                using (var emailMessage = new MailMessage())
                {
                    emailMessage.To.Add(new MailAddress(email));
                    emailMessage.From = new MailAddress("ershadrostami116@gmail.com");
                    emailMessage.Subject = subject;
                    emailMessage.IsBodyHtml = true;
                    emailMessage.Body = htmlmessage;

                    Client.Send(emailMessage);
                };

                await Task.CompletedTask;
            };
        }
    }
}
