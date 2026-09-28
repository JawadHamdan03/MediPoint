using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Users.UploadProfileImage;
using MediPoint.Common;
using MediPoint.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace MediPoint.Appliaction.SubcutaneousTests.Features.Users.UploadProfileImage;

public class UploadProfileImageCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingPatient_SavesImageAndUpdatesImageUrl()
    {
        using var dbContext = TestDbContextFactory.Create();
        var patient = TestEntities.NewPatient();
        await dbContext.Patients.AddAsync(patient);
        await dbContext.SaveChangesAsync();
        var storage = Substitute.For<IFileStorageService>();
        storage.SaveAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns("/uploads/photo.png");
        var handler = new UploadProfileImageCommandHandler(dbContext, storage);

        var result = await handler.Handle(
            new UploadProfileImageCommand(patient.Id, "Patient", new MemoryStream(), "photo.png", "image/png", 100),
            CancellationToken.None);

        Assert.Equal("/uploads/photo.png", result.ImageUrl);
        var saved = await dbContext.Patients.SingleAsync();
        Assert.Equal("/uploads/photo.png", saved.ImageUrl);
    }

    [Fact]
    public async Task Handle_ExistingDoctor_SavesImageAndUpdatesImageUrl()
    {
        using var dbContext = TestDbContextFactory.Create();
        var doctor = TestEntities.NewDoctor();
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.SaveChangesAsync();
        var storage = Substitute.For<IFileStorageService>();
        storage.SaveAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns("/uploads/photo.png");
        var handler = new UploadProfileImageCommandHandler(dbContext, storage);

        await handler.Handle(new UploadProfileImageCommand(doctor.Id, "Doctor", new MemoryStream(), "photo.png", "image/png", 100), CancellationToken.None);

        var saved = await dbContext.Doctors.SingleAsync();
        Assert.Equal("/uploads/photo.png", saved.ImageUrl);
    }

    [Fact]
    public async Task Handle_UnknownUser_ThrowsNotFoundException()
    {
        using var dbContext = TestDbContextFactory.Create();
        var handler = new UploadProfileImageCommandHandler(dbContext, Substitute.For<IFileStorageService>());

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(
            new UploadProfileImageCommand(Guid.NewGuid(), "Patient", new MemoryStream(), "photo.png", "image/png", 100),
            CancellationToken.None));
    }
}
