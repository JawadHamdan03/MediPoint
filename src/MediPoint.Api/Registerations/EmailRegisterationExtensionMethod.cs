using MediPoint.Application.Common.Services;
using MediPoint.Infrastructure.Common.Services;
using MediPoint.Infrastructure.Common.Utils;

namespace MediPoint.Api.Registerations;

public static class EmailRegisterationExtensionMethod
{
   public static IServiceCollection AddEmailRegisteration(this IServiceCollection service,IConfiguration configuration)
   {
      service.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
      service.AddTransient<IMailer,Mailer>();                                           
      return service;
   }
}