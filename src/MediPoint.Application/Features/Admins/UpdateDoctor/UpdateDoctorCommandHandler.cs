using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Admins.AdminAddsDoctor.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Admins.UpdateDoctor;

public class UpdateDoctorCommandHandler(IAppDbContext dbContext) : IRequestHandler<UpdateDoctorCommand, DoctorDto>
{
    public async Task<DoctorDto> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await dbContext.Doctors.FirstOrDefaultAsync(d => d.Id == request.DoctorId);

        if (doctor is null)
            throw new NotFoundException("Doctor", request.DoctorId.ToString());

        request.Doctor.Adapt(doctor);
        await dbContext.SaveChangesAsync(cancellationToken);

        return doctor.Adapt<DoctorDto>();
    }
}
