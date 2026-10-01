using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Users.GetPrescriptionPdf;
using MediPoint.Common;
using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.Prescriptions;
using MediPoint.Domain.Entities.Prescriptions.LabRes;
using MediPoint.Domain.Entities.Prescriptions.Med;
using NSubstitute;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Users.GetPrescriptionPdf;

public class GetPrescriptionPdfQueryHandlerTests
{
    private static (MediPoint.Infrastructure.Data.AppDbContext dbContext, Prescription prescription) SeedPrescription()
    {
        var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        var patient = TestEntities.NewPatient();
        var appointment = Appointment.Create(doctor.Id, DateTime.UtcNow.AddDays(-1), 30);
        appointment.Doctor = doctor;
        appointment.Confirm(patient.Id);
        appointment.Complete(null);

        var prescription = new Prescription
        {
            Diagnosis = "Seasonal allergy",
            Notes = "Follow up in 2 weeks",
            PatientId = patient.Id,
            DoctorId = doctor.Id,
            AppointmentId = appointment.Id,
        };

        dbContext.Doctors.Add(doctor);
        dbContext.Patients.Add(patient);
        dbContext.Appointments.Add(appointment);
        dbContext.Prescriptions.Add(prescription);
        dbContext.SaveChanges();

        return (dbContext, prescription);
    }

    [Fact]
    public async Task Handle_OwningPatient_GeneratesPdfWithCorrectModel()
    {
        var (dbContext, prescription) = SeedPrescription();
        using var _ = dbContext;
        var medicineService = Substitute.For<IMedicineService>();
        medicineService.GetAsync().Returns(new List<Medicine>
        {
            new() { PrescriptionId = prescription.Id, Name = "Loratadine", Dosage = "10mg", Frequency = "Once daily", DurationDays = 7 },
        });
        var labResultService = Substitute.For<ILabResultService>();
        labResultService.GetAsync().Returns(new List<LabResult>());
        var pdfGenerator = Substitute.For<IPrescriptionPdfGenerator>();
        pdfGenerator.Generate(Arg.Any<PrescriptionPdfModel>()).Returns(new byte[] { 1, 2, 3 });
        var handler = new GetPrescriptionPdfQueryHandler(dbContext, medicineService, labResultService, pdfGenerator);

        var result = await handler.Handle(new GetPrescriptionPdfQuery(prescription.Id, prescription.PatientId, "Patient"), CancellationToken.None);

        Assert.Equal(new byte[] { 1, 2, 3 }, result);
        pdfGenerator.Received(1).Generate(Arg.Is<PrescriptionPdfModel>(m =>
            m.Diagnosis == "Seasonal allergy" &&
            m.Medicines.Count == 1 &&
            m.Medicines[0].Name == "Loratadine" &&
            m.LabResults.Count == 0));
    }

    [Fact]
    public async Task Handle_OwningDoctor_Succeeds()
    {
        var (dbContext, prescription) = SeedPrescription();
        using var _ = dbContext;
        var medicineService = Substitute.For<IMedicineService>();
        medicineService.GetAsync().Returns(new List<Medicine>());
        var labResultService = Substitute.For<ILabResultService>();
        labResultService.GetAsync().Returns(new List<LabResult>());
        var pdfGenerator = Substitute.For<IPrescriptionPdfGenerator>();
        pdfGenerator.Generate(Arg.Any<PrescriptionPdfModel>()).Returns(new byte[] { 9 });
        var handler = new GetPrescriptionPdfQueryHandler(dbContext, medicineService, labResultService, pdfGenerator);

        var result = await handler.Handle(new GetPrescriptionPdfQuery(prescription.Id, prescription.DoctorId, "Doctor"), CancellationToken.None);

        Assert.Equal(new byte[] { 9 }, result);
    }

    [Fact]
    public async Task Handle_NonOwningPatient_ThrowsNotFoundException()
    {
        var (dbContext, prescription) = SeedPrescription();
        using var _ = dbContext;
        var handler = new GetPrescriptionPdfQueryHandler(
            dbContext, Substitute.For<IMedicineService>(), Substitute.For<ILabResultService>(), Substitute.For<IPrescriptionPdfGenerator>());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GetPrescriptionPdfQuery(prescription.Id, Guid.NewGuid(), "Patient"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UnsupportedRole_ThrowsNotFoundException()
    {
        var (dbContext, prescription) = SeedPrescription();
        using var _ = dbContext;
        var handler = new GetPrescriptionPdfQueryHandler(
            dbContext, Substitute.For<IMedicineService>(), Substitute.For<ILabResultService>(), Substitute.For<IPrescriptionPdfGenerator>());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GetPrescriptionPdfQuery(prescription.Id, Guid.NewGuid(), "Admin"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UnknownPrescription_ThrowsNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var handler = new GetPrescriptionPdfQueryHandler(
            dbContext, Substitute.For<IMedicineService>(), Substitute.For<ILabResultService>(), Substitute.For<IPrescriptionPdfGenerator>());

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GetPrescriptionPdfQuery(Guid.NewGuid(), Guid.NewGuid(), "Patient"), CancellationToken.None));
    }
}
