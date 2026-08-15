using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Admins.AdminAddsDoctor.DTOs;
using MediPoint.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Admins.AdminAddsDoctor;

public class AddDoctorCommandHandler(IAppDbContext dbContext) : IRequestHandler<AddDoctorCommand,DoctorDto>
{
    public async Task<DoctorDto> Handle(AddDoctorCommand request, CancellationToken cancellationToken)
    { 
        var res = await dbContext.Doctors.FirstOrDefaultAsync(d=>d.Email.Equals(request.doctorRequest.Email));

        if (res is not null)
        {
            throw new ConflictException("Doctor already exists");
        }

        var doc = request.doctorRequest.Adapt<Doctor>();
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.doctorRequest.Password);

        doc.PasswordHash = passwordHash;


        await dbContext.Doctors.AddAsync(doc);
        return doc.Adapt<DoctorDto>();
    }
}