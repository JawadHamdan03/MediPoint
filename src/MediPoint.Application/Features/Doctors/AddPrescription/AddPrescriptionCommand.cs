using MediatR;
using MediPoint.Application.Features.Doctors.AddPrescription.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.AddPrescription;

public sealed record AddPrescriptionCommand(PrescriptionRequest PrescriptionRequest) : IRequest<PrescriptionResponse>;