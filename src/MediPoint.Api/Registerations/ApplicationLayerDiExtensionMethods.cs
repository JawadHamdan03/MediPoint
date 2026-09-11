using FluentValidation;
using MediatR;
using MediPoint.Application;
using MediPoint.Application.Common.Behaviors;

namespace MediPoint.Api.Registerations;

public static class ApplicationLayerDiExtensionMethods
{
    public static IServiceCollection AddApplicationLayerRegisteration(this IServiceCollection service)
    {
        service.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(typeof(IAssemblyMarker).Assembly);
        });

        service.AddValidatorsFromAssembly(typeof(IAssemblyMarker).Assembly);
        service.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return service;
    }
}