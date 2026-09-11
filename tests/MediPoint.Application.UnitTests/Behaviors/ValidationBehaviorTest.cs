using FluentValidation;
using MediatR;
using MediPoint.Application.Common.Behaviors;
using MediPoint.Application.Features.Admins.AdminAddsDoctor;
using MediPoint.Application.Features.Admins.AdminAddsDoctor.DTOs;
using NSubstitute;
using Xunit;

namespace MediPoint.Application.UnitTests.Behaviors;

public class ValidationBehaviorTest
{
    private readonly ValidationBehavior<AddDoctorCommand, DoctorDto> _validationBehavior;
    private readonly IEnumerable<IValidator<AddDoctorCommand>> _mockValidator;
    private readonly RequestHandlerDelegate<DoctorDto> _mockNextBehavior;

    public ValidationBehaviorTest()
    {
        _mockNextBehavior = Substitute.For<RequestHandlerDelegate<DoctorDto>>();
        _mockValidator = Substitute.For<IEnumerable<IValidator<AddDoctorCommand>>>();
        
        _validationBehavior = new(_mockValidator);
    }


    [Fact]
    public async Task InvokeValidationBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
    {
        // Arrange 
        
        // Act 
        
        // Assert
    }
}