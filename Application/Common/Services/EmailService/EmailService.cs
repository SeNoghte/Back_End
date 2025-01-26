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
        IConfiguration config;
        string emailHost;
        string emailUserName;
        string emailPassword;
        string senderEmailAddress;
        public EmailService(IConfiguration _config)
        {
            config = _config;
            emailUserName = config.GetSection("EmailService:EmailUsername").Value!;
            emailHost = config.GetSection("EmailService:EmailHost").Value!;
            emailPassword = config.GetSection("EmailService:EmailPassword").Value!;
            senderEmailAddress = config.GetSection("EmailService:SenderEmailAddress").Value!;
        }

        public async Task SendMail(string email, string subject, string body)
        {
            var emailModel = new MimeMessage();

            emailModel.From.Add(new MailboxAddress("بچین", senderEmailAddress));
            emailModel.To.Add(MailboxAddress.Parse(email));
            emailModel.Subject = subject;
            emailModel.Body = new TextPart(TextFormat.Html) { Text = body };

            using SmtpClient client = new(new ProtocolLogger("smtp.log"));
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync(emailHost, 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(emailUserName, emailPassword);
            await client.SendAsync(emailModel);
            await client.DisconnectAsync(true);
        }
    }
}
