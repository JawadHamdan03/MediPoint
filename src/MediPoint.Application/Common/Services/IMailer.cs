namespace MediPoint.Application.Common.Services;

public interface IMailer
{
   Task SendEmailAsync(string email,string subject, string body);
}