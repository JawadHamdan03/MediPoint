using MediPoint.Application.Common.Services;
using MediPoint.Domain.Entities.Appointments.Enums;
using MediPoint.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediPoint.Infrastructure.Common.Jobs;

public class AppointmentReminderJob(IServiceScopeFactory _scopeFactory,IMailer mailer,ILogger<AppointmentReminderJob> _logger) : BackgroundService
{
   private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(1);
   private static readonly TimeSpan ReminderWindowStart = TimeSpan.FromHours(23);
   private static readonly TimeSpan ReminderWindowEnd = TimeSpan.FromHours(24);

   protected override async Task ExecuteAsync(CancellationToken stoppingToken)
   {
      try
      {
         while (!stoppingToken.IsCancellationRequested)
         {
            _logger.LogInformation("AppointmentReminderJob running at {time}", DateTime.UtcNow);

            using (var scope = _scopeFactory.CreateScope())
            {
               var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

               var now = DateTime.UtcNow;
               var windowStart = now + ReminderWindowStart;
               var windowEnd = now + ReminderWindowEnd;

               var upcomingAppointments = await dbContext.Appointments
                  .Include(a => a.Patient)
                  .Include(a => a.Doctor)
                  .Where(a => a.Status == AppointmentStatus.Confirmed
                     && a.AppointmentDate >= windowStart
                     && a.AppointmentDate < windowEnd)
                  .ToListAsync(stoppingToken);

               foreach (var appointment in upcomingAppointments)
               {
                  if (appointment.Patient is null)
                     continue;

                  await mailer.SendEmailAsync(appointment.Patient.Email, "Appointment Reminder",
                     $"Hi {appointment.Patient.FirstName}, this is a reminder that you have an appointment with Dr. {appointment.Doctor.FirstName} {appointment.Doctor.LastName} on {appointment.AppointmentDate:f}.");
               }
            }

            await Task.Delay(CheckInterval, stoppingToken);
         }
      }
      catch (OperationCanceledException)
      {
         _logger.LogInformation("AppointmentReminderJob stopping as OperationCanceledException exception thrown");
      }
      finally
      {
         _logger.LogInformation("AppointmentReminderJob stopping");
      }
   }
}
