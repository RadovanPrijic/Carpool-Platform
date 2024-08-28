namespace CarpoolPlatformAPI.Util.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
    }
}
