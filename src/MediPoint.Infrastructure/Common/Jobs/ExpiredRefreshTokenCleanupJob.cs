using MediPoint.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediPoint.Infrastructure.Common.Jobs;

public class ExpiredRefreshTokenCleanupJob(IServiceScopeFactory _scopeFactory,ILogger<ExpiredRefreshTokenCleanupJob> _logger) : BackgroundService
{
   private static readonly TimeSpan CheckInterval = TimeSpan.FromDays(1);

   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      try
      {
         while (!stoppingToken.IsCancellationRequested)
         {
            _logger.LogInformation("ExpiredRefreshTokenCleanupJob running at {time}", DateTime.UtcNow);

            using (var scope = _scopeFactory.CreateScope())
            {
               var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
               var now = DateTime.UtcNow;

               var removedAdminTokens = await dbContext.AdminRefreshTokens
                  .Where(t => t.ExpiresAt < now)
                  .ExecuteDeleteAsync(stoppingToken);

               var removedDoctorTokens = await dbContext.DoctorRefreshTokens
                  .Where(t => t.ExpiresAt < now)
                  .ExecuteDeleteAsync(stoppingToken);

               var removedPatientTokens = await dbContext.PatientRefreshTokens
                  .Where(t => t.ExpiresAt < now)
                  .ExecuteDeleteAsync(stoppingToken);

               _logger.LogInformation(
                  "Removed {AdminCount} admin, {DoctorCount} doctor and {PatientCount} patient expired refresh tokens",
                  removedAdminTokens, removedDoctorTokens, removedPatientTokens);
            }

            await Task.Delay(CheckInterval, stoppingToken);
         }
      }
      catch (OperationCanceledException)
      {
         _logger.LogInformation("ExpiredRefreshTokenCleanupJob stopping as OperationCanceledException exception thrown");
      }
      finally
      {
         _logger.LogInformation("ExpiredRefreshTokenCleanupJob stopping");
      }
   }
}
