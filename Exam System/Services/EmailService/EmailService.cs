using System.Net;
using System.Net.Mail;

namespace ExamSystem.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            //configure smtp client
            var smtpClient = new SmtpClient()
            {
                Host = configuration["EmailSettings:SmtpServer"],
                Port = int.Parse(configuration["EmailSettings:Port"]),
                Credentials = new NetworkCredential(configuration["EmailSettings:SenderEmail"], configuration["EmailSettings:Password"]),
                EnableSsl = true,
                UseDefaultCredentials = false

            };

            //create email message
            var message = new MailMessage()
            {
                From = new MailAddress(configuration["EmailSettings:SenderEmail"], configuration["EmailSettings:SenderName"]),
                Body= body,
                Subject = subject,
                IsBodyHtml=true
            };

            message.To.Add(to);

            //send message
            await smtpClient.SendMailAsync(message);

            //logging in console
            Console.WriteLine($"Email sent to {to} with {subject}");

           

        }
    }
}
