using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.Services;
using MediPoint.Application.Features.Admins.AdminAddsDoctor.DTOs;
using MediPoint.Domain.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Admins.RemoveDoctor;

public class RemoveDoctorCommandHandler(IAppDbContext dbContext,IMailer mailer) : IRequestHandler<RemoveDoctorCommand, DoctorDto>
{
    public async Task<DoctorDto> Handle(RemoveDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await dbContext.Doctors.FirstOrDefaultAsync(d => d.Id == request.DoctorId);

        if (doctor is null)
            throw new NotFoundException("Doctor", request.DoctorId.ToString());

        try
        {
            doctor.Deactivate();
        }
        catch (DomainException ex)
        {
            throw new ConflictException(ex.Message);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        await mailer.SendEmailAsync(doctor.Email, "Account Deactivated",
            $"Hi Dr. {doctor.FirstName}, your account has been deactivated by an administrator.");

        return doctor.Adapt<DoctorDto>();
    }
}
