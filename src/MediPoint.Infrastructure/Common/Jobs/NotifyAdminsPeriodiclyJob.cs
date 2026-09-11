using MediPoint.Application.Common;
using MediPoint.Application.Common.Services;
using MediPoint.Domain.Entities.User;
using MediPoint.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediPoint.Infrastructure.Common.Jobs;

public class NotifyAdminsPeriodiclyJob(IServiceScopeFactory _scopeFactory,IMailer mailer,ILogger<NotifyAdminsPeriodiclyJob> _logger) : BackgroundService
{
   protected async override Task ExecuteAsync(CancellationToken stoppingToken)
   {
      try
      {
         while (!stoppingToken.IsCancellationRequested)
         {
            _logger.LogInformation("Worker running at {time}", DateTime.UtcNow);
            List<Admin> admins;
            using (var scope = _scopeFactory.CreateScope())
            {
               var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
               admins = await dbContext.Admins.ToListAsync();
            } 
            foreach (var admin in admins)
            {
               await mailer.SendEmailAsync(admin.Email,"Notify",$"Please admin {admin.FirstName}, check on the system.");
            }
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
         }
      }
      catch (OperationCanceledException)
      {
     
         _logger.LogInformation("Worker stopping as OperationCanceledException exception thrown");
      }
      finally
      {
         _logger.LogInformation("Worker stopping");
      }
   }
}