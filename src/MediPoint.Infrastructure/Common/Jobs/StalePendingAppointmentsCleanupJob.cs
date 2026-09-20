using MediPoint.Application.Common.Services;
using MediPoint.Domain.Entities.Appointments.Enums;
using MediPoint.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediPoint.Infrastructure.Common.Jobs;

public class StalePendingAppointmentsCleanupJob(IServiceScopeFactory _scopeFactory,IMailer mailer,ILogger<StalePendingAppointmentsCleanupJob> _logger) : BackgroundService
{
   private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(6);

   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      try
      {
         while (!stoppingToken.IsCancellationRequested)
         {
            _logger.LogInformation("StalePendingAppointmentsCleanupJob running at {time}", DateTime.UtcNow);

            using (var scope = _scopeFactory.CreateScope())
            {
               var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
               var now = DateTime.UtcNow;

               var staleAppointments = await dbContext.Appointments
                  .Include(a => a.Doctor)
                  .Where(a => a.Status == AppointmentStatus.Pending && a.AppointmentDate < now)
                  .ToListAsync(stoppingToken);

               foreach (var appointment in staleAppointments)
               {
                  appointment.Cancel("Automatically cancelled: slot expired without being booked.");
               }

               if (staleAppointments.Count > 0)
                  await dbContext.SaveChangesAsync(stoppingToken);

               _logger.LogInformation("Auto-cancelled {Count} stale pending appointments", staleAppointments.Count);

               foreach (var appointment in staleAppointments)
               {
                  await mailer.SendEmailAsync(appointment.Doctor.Email, "Appointment Slot Expired",
                     $"Hi Dr. {appointment.Doctor.FirstName}, your open appointment slot on {appointment.AppointmentDate:f} expired without being booked and was automatically cancelled.");
               }
            }

            await Task.Delay(CheckInterval, stoppingToken);
         }
      }
      catch (OperationCanceledException)
      {
         _logger.LogInformation("StalePendingAppointmentsCleanupJob stopping as OperationCanceledException exception thrown");
      }
      finally
      {
         _logger.LogInformation("StalePendingAppointmentsCleanupJob stopping");
      }
   }
}
