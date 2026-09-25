using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using MediPoint.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Doctors.SignUp;

public class SignUpDoctorCommandHandler(IAppDbContext dbContext, IJwtTokenServiceProvider jwtTokenServiceProvider)
    : IRequestHandler<SignUpDoctorCommand, JwtTokenResponse>
{
    public async Task<JwtTokenResponse> Handle(SignUpDoctorCommand request, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Doctors.FirstOrDefaultAsync(d => d.Email.Equals(request.Request.Email), cancellationToken);

        if (existing is not null)
        {
            throw new ConflictException("Doctor already exists");
        }

        var doctor = request.Request.Adapt<Doctor>();
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.Password);

        doctor.PasswordHash = passwordHash;

        await dbContext.Doctors.AddAsync(doctor, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await jwtTokenServiceProvider.GenerateJwtToken(doctor);
    }
}
