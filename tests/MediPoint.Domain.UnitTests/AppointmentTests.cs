using MediPoint.Domain.Common.Exceptions;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.Appointments.Enums;
using Xunit;

namespace MediPoint.Domain.UnitTests;

public class AppointmentTests
{
    private static readonly Guid DoctorId = Guid.NewGuid();
    private static readonly Guid PatientId = Guid.NewGuid();
    private static readonly DateTime FutureDate = DateTime.UtcNow.AddDays(1);

    [Fact]
    public void Create_WithValidDuration_ReturnsPendingAppointment()
    {
        var appointment = Appointment.Create(DoctorId, FutureDate, 30, "Checkup");

        Assert.Equal(DoctorId, appointment.DoctorId);
        Assert.Equal(FutureDate, appointment.AppointmentDate);
        Assert.Equal(30, appointment.Duration);
        Assert.Equal("Checkup", appointment.Reason);
        Assert.Equal(AppointmentStatus.Pending, appointment.Status);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-15)]
    public void Create_WithNonPositiveDuration_ThrowsDomainException(int duration)
    {
        Assert.Throws<DomainException>(() => Appointment.Create(DoctorId, FutureDate, duration));
    }

    [Fact]
    public void Confirm_WhenPending_SetsPatientAndConfirmedStatus()
    {
        var appointment = Appointment.Create(DoctorId, FutureDate, 30);

        appointment.Confirm(PatientId);

        Assert.Equal(PatientId, appointment.PatientId);
        Assert.Equal(AppointmentStatus.Confirmed, appointment.Status);
    }

    [Fact]
    public void Confirm_WhenAlreadyConfirmed_ThrowsDomainException()
    {
        var appointment = Appointment.Create(DoctorId, FutureDate, 30);
        appointment.Confirm(PatientId);

        Assert.Throws<DomainException>(() => appointment.Confirm(Guid.NewGuid()));
    }

    [Fact]
    public void Confirm_WhenCancelled_ThrowsDomainException()
    {
        var appointment = Appointment.Create(DoctorId, FutureDate, 30);
        appointment.Cancel("no longer needed");

        Assert.Throws<DomainException>(() => appointment.Confirm(PatientId));
    }

    [Fact]
    public void Confirm_WhenCompleted_ThrowsDomainException()
    {
        var appointment = Appointment.Create(DoctorId, FutureDate, 30);
        appointment.Confirm(PatientId);
        appointment.Complete(null);

        Assert.Throws<DomainException>(() => appointment.Confirm(Guid.NewGuid()));
    }

    [Fact]
    public void Cancel_WhenPending_SetsCancelledStatusAndReason()
    {
        var appointment = Appointment.Create(DoctorId, FutureDate, 30);

        appointment.Cancel("Patient request");

        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
        Assert.Equal("Patient request", appointment.CancellationReason);
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ThrowsDomainException()
    {
        var appointment = Appointment.Create(DoctorId, FutureDate, 30);
        appointment.Cancel("first cancel");

        Assert.Throws<DomainException>(() => appointment.Cancel("second cancel"));
    }

    [Fact]
    public void Cancel_WhenCompleted_ThrowsDomainException()
    {
        var appointment = Appointment.Create(DoctorId, FutureDate, 30);
        appointment.Confirm(PatientId);
        appointment.Complete(null);

        Assert.Throws<DomainException>(() => appointment.Cancel("too late"));
    }

    [Fact]
    public void Complete_WhenConfirmed_SetsCompletedStatusAndNotes()
    {
        var appointment = Appointment.Create(DoctorId, FutureDate, 30);
        appointment.Confirm(PatientId);

        appointment.Complete("All good");

        Assert.Equal(AppointmentStatus.Completed, appointment.Status);
        Assert.Equal("All good", appointment.Notes);
    }

    [Fact]
    public void Complete_WithNullNotes_LeavesExistingNotesUnchanged()
    {
        var appointment = Appointment.Create(DoctorId, FutureDate, 30);
        appointment.Confirm(PatientId);

        appointment.Complete(null);

        Assert.Null(appointment.Notes);
    }

    [Fact]
    public void Complete_WhenPending_ThrowsDomainException()
    {
        var appointment = Appointment.Create(DoctorId, FutureDate, 30);

        Assert.Throws<DomainException>(() => appointment.Complete("notes"));
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_ThrowsDomainException()
    {
        var appointment = Appointment.Create(DoctorId, FutureDate, 30);
        appointment.Confirm(PatientId);
        appointment.Complete("done");

        Assert.Throws<DomainException>(() => appointment.Complete("done again"));
    }
}
