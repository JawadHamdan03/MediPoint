using MediPoint.Application.Common.Services;
using MediPoint.Infrastructure.Common.Services;

namespace MediPoint.Api.Registerations;

public static class StorageRegisterations
{
   public static IServiceCollection AddStorageRegisteration(this IServiceCollection service)
   {
      service.AddScoped<IFileStorageService, LocalFileStorageService>();
      return service;
   }
}
