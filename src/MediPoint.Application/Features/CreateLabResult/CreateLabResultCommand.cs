using MediatR;
using MediPoint.Application.Features.CreateLabResult.DTOs;
using MediPoint.Domain.Entities.Prescriptions.LabRes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.CreateLabResult;

public sealed record CreateLabResultCommand(LabResultRequest Request) : IRequest<LabResult>;
