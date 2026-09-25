using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Common.ServiceResponse;
using MediPoint.Application.Common.Services;
using MediPoint.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Patients.SignUp;

public class SignUpPatientCommandHandler(IAppDbContext dbContext, IJwtTokenServiceProvider jwtTokenServiceProvider)
    : IRequestHandler<SignUpPatientCommand, JwtTokenResponse>
{
    public async Task<JwtTokenResponse> Handle(SignUpPatientCommand request, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Patients.FirstOrDefaultAsync(p => p.Email.Equals(request.Request.Email), cancellationToken);

        if (existing is not null)
        {
            throw new ConflictException("Patient already exists");
        }

        var patient = request.Request.Adapt<Patient>();
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.Password);

        patient.PasswordHash = passwordHash;

        await dbContext.Patients.AddAsync(patient, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await jwtTokenServiceProvider.GenerateJwtToken(patient);
    }
}
