using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PriceTracker.Services
{
    public static class EmailSender
    {
        public static async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("dhruvyadav2905@gmail.com", "gtyj mbeo foky rprc"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("your_email@gmail.com", "PriceTracker App"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
