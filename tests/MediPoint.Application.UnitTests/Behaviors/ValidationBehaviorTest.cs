using FluentValidation;
using FluentValidation.Results;
using MediatR;
using MediPoint.Application.Common.Behaviors;
using MediPoint.Application.Features.Admins.AdminAddsDoctor;
using MediPoint.Application.Features.Admins.AdminAddsDoctor.DTOs;
using NSubstitute;
using Xunit;

namespace MediPoint.Application.UnitTests.Behaviors;

public class ValidationBehaviorTest
{
    private readonly AddDoctorCommand _command = new(new DoctorDto());
    private readonly RequestHandlerDelegate<DoctorDto> _mockNextBehavior;
    private readonly DoctorDto _expectedResponse = new();

    public ValidationBehaviorTest()
    {
        _mockNextBehavior = Substitute.For<RequestHandlerDelegate<DoctorDto>>();
        _mockNextBehavior.Invoke(Arg.Any<CancellationToken>()).Returns(_expectedResponse);
    }

    [Fact]
    public async Task InvokeValidationBehavior_WhenNoValidatorsRegistered_ShouldInvokeNextBehavior()
    {
        // Arrange
        var validators = new List<IValidator<AddDoctorCommand>>();
        var validationBehavior = new ValidationBehavior<AddDoctorCommand, DoctorDto>(validators);

        // Act
        var result = await validationBehavior.Handle(_command, _mockNextBehavior, CancellationToken.None);

        // Assert
        Assert.Same(_expectedResponse, result);
        await _mockNextBehavior.Received(1).Invoke(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InvokeValidationBehavior_WhenValidatorResultIsValid_ShouldInvokeNextBehavior()
    {
        // Arrange
        var mockValidator = Substitute.For<IValidator<AddDoctorCommand>>();
        mockValidator
            .ValidateAsync(Arg.Any<ValidationContext<AddDoctorCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var validationBehavior = new ValidationBehavior<AddDoctorCommand, DoctorDto>([mockValidator]);

        // Act
        var result = await validationBehavior.Handle(_command, _mockNextBehavior, CancellationToken.None);

        // Assert
        Assert.Same(_expectedResponse, result);
        await _mockNextBehavior.Received(1).Invoke(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InvokeValidationBehavior_WhenValidatorResultIsInvalid_ShouldThrowValidationExceptionAndNotInvokeNextBehavior()
    {
        // Arrange
        var failure = new ValidationFailure("FirstName", "First name is required");
        var mockValidator = Substitute.For<IValidator<AddDoctorCommand>>();
        mockValidator
            .ValidateAsync(Arg.Any<ValidationContext<AddDoctorCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([failure]));

        var validationBehavior = new ValidationBehavior<AddDoctorCommand, DoctorDto>([mockValidator]);

        // Act
        var act = () => validationBehavior.Handle(_command, _mockNextBehavior, CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.Contains(exception.Errors, e => e.PropertyName == "FirstName");
        await _mockNextBehavior.DidNotReceive().Invoke(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InvokeValidationBehavior_WhenMultipleValidatorsAndOneFails_ShouldThrowValidationExceptionWithAggregatedErrors()
    {
        // Arrange
        var passingValidator = Substitute.For<IValidator<AddDoctorCommand>>();
        passingValidator
            .ValidateAsync(Arg.Any<ValidationContext<AddDoctorCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var failingValidator = Substitute.For<IValidator<AddDoctorCommand>>();
        failingValidator
            .ValidateAsync(Arg.Any<ValidationContext<AddDoctorCommand>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("Email", "Email is invalid")]));

        var validationBehavior = new ValidationBehavior<AddDoctorCommand, DoctorDto>([passingValidator, failingValidator]);

        // Act
        var act = () => validationBehavior.Handle(_command, _mockNextBehavior, CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.Single(exception.Errors);
        await _mockNextBehavior.DidNotReceive().Invoke(Arg.Any<CancellationToken>());
    }
}
