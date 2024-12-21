using System.Net.Mail;
using System.Net;

namespace WebBanGear.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("ngoson17102003@gmail.com", "doqjzxberuuoyeuo")
            };

            return client.SendMailAsync(
                new MailMessage(from: "ngoson17102003@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
