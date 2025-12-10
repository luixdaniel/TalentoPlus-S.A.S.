namespace ApiTalento.Web.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string employeeName);
    }
}

