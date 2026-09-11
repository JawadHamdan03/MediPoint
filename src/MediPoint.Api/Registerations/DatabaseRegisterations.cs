using MediPoint.Application.Common;
using MediPoint.Infrastructure.Data;
using MediPoint.Infrastructure.MongoData;
using MediPoint.Infrastructure.MongoData.Services;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Api.Registerations;

public static class DatabaseRegisterations
{
   public static IServiceCollection AddDbRegisteration(this IServiceCollection service,IConfiguration configuration)
   {
      service.AddDbContext<AppDbContext>(options =>
      {
         options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
      });

      service.AddScoped<IAppDbContext, AppDbContext>();
      service.Configure<MongoDbContext>(configuration.GetSection("MediPoint"));
      service.AddSingleton<IMedicalRecordsService,MedicalRecordService>();
      service.AddSingleton<IMedicineService,MedicineService>();
      service.AddSingleton<ILabResultService,LabResultService>();
      
      return service;
   }
}