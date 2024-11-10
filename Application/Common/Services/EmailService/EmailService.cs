using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;


namespace Application.Common.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMail(string email, string subject, string body)
        {
            var emailModel = new MimeMessage();

            emailModel.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailService:EmailUsername").Value));
            emailModel.To.Add(MailboxAddress.Parse(email));
            emailModel.Subject = subject;
            emailModel.Body = new TextPart(TextFormat.Html) { Text = body };

            using SmtpClient client = new(new ProtocolLogger("smtp.log"));
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync(_configuration.GetSection("EmailService:EmailHost").Value, 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_configuration.GetSection("EmailService:EmailUsername").Value, _configuration.GetSection("EmailService:EmailPassword").Value);
            await client.SendAsync(emailModel);
            await client.DisconnectAsync(true);
        }
    }
}
