using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Admins.AdminAddsDoctor.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Admins.RemoveDoctor;

public class RemoveDoctorCommandHandler(IAppDbContext dbContext) : IRequestHandler<RemoveDoctorCommand, DoctorDto>
{
    public async Task<DoctorDto> Handle(RemoveDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await dbContext.Doctors.FirstOrDefaultAsync(d => d.Id == request.DoctorId);

        if (doctor is null)
            throw new NotFoundException("Doctor", request.DoctorId.ToString());

        if (!doctor.IsAvailable)
            throw new ConflictException("Doctor is already removed");

        doctor.IsAvailable = false;
        await dbContext.SaveChangesAsync(cancellationToken);

        return doctor.Adapt<DoctorDto>();
    }
}
