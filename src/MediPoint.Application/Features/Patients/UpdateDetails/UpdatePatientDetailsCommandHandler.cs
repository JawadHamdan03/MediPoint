using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Patients.UpdateDetails.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MediPoint.Application.Features.Patients.UpdateDetails;

public class UpdatePatientDetailsCommandHandler(IAppDbContext dbContext) : IRequestHandler<UpdatePatientDetailsCommand, UpdatePatientDto>
{
    public async Task<UpdatePatientDto> Handle(UpdatePatientDetailsCommand request, CancellationToken cancellationToken)
    {
        var patient = await dbContext.Patients.FirstOrDefaultAsync(p => p.Id == request.PatientId);

        if (patient is null)
            throw new NotFoundException("Patient", request.PatientId.ToString());

        request.Details.Adapt(patient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return patient.Adapt<UpdatePatientDto>();
    }
}
