using MailKit.Net.Smtp;
using MailKit.Security;
using MediPoint.Application.Common.Services;
using MediPoint.Infrastructure.Common.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MediPoint.Infrastructure.Common.Services;

public class Mailer : IMailer
{
   private readonly SmtpSettings _smtpSettings;
   private readonly IWebHostEnvironment _env;

   public Mailer(IOptions<SmtpSettings> options, IWebHostEnvironment env)
   {
      _smtpSettings = options.Value;
      _env = env;
   }

   public async Task SendEmailAsync(string email, string subject, string body)
   {
      try
      {
         var message = new MimeMessage();

         message.From.Add(
            new MailboxAddress(
               _smtpSettings.SenderName,
               _smtpSettings.SenderEmail
            )
         );

         message.To.Add(new MailboxAddress("user", email));
         message.Subject = subject;

         message.Body = new TextPart("html")
         {
            Text = body
         };

         using var client = new SmtpClient();

         if (_env.IsDevelopment())
         {
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
         }

         // SecureSocketOptions.StartTls handles port 587 correctly in all environments
         await client.ConnectAsync(
            _smtpSettings.Server,
            _smtpSettings.Port,
            SecureSocketOptions.StartTls
         );

         await client.AuthenticateAsync(
            _smtpSettings.Username,
            _smtpSettings.Password
         );

         await client.SendAsync(message);
         await client.DisconnectAsync(true);
      }
      catch (Exception e)
      {
         // Preserve original exception details inside the inner exception
         throw new InvalidOperationException($"Email delivery failed: {e.Message}", e);
      }
   }
}