using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Admins.RegisterPatient.DTOs;
using MediPoint.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Admins.RegisterPatient;

public class RegisterPatientCommandHandler(IAppDbContext dbContext) : IRequestHandler<RegisterPatientCommand, PatientDto>
{
    public async Task<PatientDto> Handle(RegisterPatientCommand request, CancellationToken cancellationToken)
    {
        var existing = await dbContext.Patients.FirstOrDefaultAsync(p => p.Email.Equals(request.patientRequest.Email));

        if (existing is not null)
        {
            throw new ConflictException("Patient already exists");
        }

        var patient = request.patientRequest.Adapt<Patient>();
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.patientRequest.Password);

        patient.PasswordHash = passwordHash;

        await dbContext.Patients.AddAsync(patient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return patient.Adapt<PatientDto>();
    }
}
