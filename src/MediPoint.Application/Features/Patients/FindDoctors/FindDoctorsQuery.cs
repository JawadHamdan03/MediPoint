using MediatR;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.FindDoctors;

public sealed record FindDoctorsQuery(string speciality) : IRequest<List<DoctorResponse>>;
