using MediPoint.Domain.Common.Exceptions;
using MediPoint.Domain.Entities.User;
using Xunit;

namespace MediPoint.Domain.UnitTests;

public class DoctorTests
{
    [Fact]
    public void NewDoctor_IsAvailableByDefault()
    {
        var doctor = new Doctor();

        Assert.True(doctor.IsAvailable);
    }

    [Fact]
    public void Deactivate_WhenAvailable_SetsIsAvailableFalse()
    {
        var doctor = new Doctor();

        doctor.Deactivate();

        Assert.False(doctor.IsAvailable);
    }

    [Fact]
    public void Deactivate_WhenAlreadyDeactivated_ThrowsDomainException()
    {
        var doctor = new Doctor();
        doctor.Deactivate();

        Assert.Throws<DomainException>(() => doctor.Deactivate());
    }
}
