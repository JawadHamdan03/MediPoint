using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Domain.Entities.Prescriptions.LabRes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.CreateLabResult;

public class CreateLabResultCommandHandler(ILabResultService labResultService) : IRequestHandler<CreateLabResultCommand, LabResult>
{
    public async Task<LabResult> Handle(CreateLabResultCommand request, CancellationToken cancellationToken)
    {
        var labRes = request.Request.Adapt<LabResult>();
        await labResultService.CreateAsync(labRes);
        return labRes;
    }
}
