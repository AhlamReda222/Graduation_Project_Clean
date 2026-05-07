namespace Graduation_Project.BLL.Services.Interfaces
{
    public interface IEmailSender
    {

        Task SendEmailAsync(string toEmail, string subject, string body);

    }
}
