using MediPoint.Api.Exceptions;
using Microsoft.AspNetCore.Http.Features;

namespace MediPoint.Api.Registerations;

public static class ExceptionHandlingRegisterationsExtensionMethods
{
   public static IServiceCollection AddExceptionHandlingRegisertations(this IServiceCollection service)
   {
      service.AddProblemDetails(options =>
      {
         options.CustomizeProblemDetails = context =>
         {
            context.ProblemDetails.Instance =
               $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

            context.ProblemDetails.Extensions.TryAdd("requestId",context.HttpContext.TraceIdentifier);
            var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
            context.ProblemDetails.Extensions.TryAdd("traceId",activity?.Id);
         };
      });

      service.AddExceptionHandler<GlobalExceptionHandler>();

      return service;
   }
}