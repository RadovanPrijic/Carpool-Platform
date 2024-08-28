using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;

namespace CarpoolPlatformAPI.Util.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
                emailMessage.To.Add(new MailboxAddress("", toEmail));
                emailMessage.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = isHtml ? body : null, TextBody = !isHtml ? body : null };
                emailMessage.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, _smtpSettings.EnableSsl);
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
