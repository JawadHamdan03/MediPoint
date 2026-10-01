using MediatR;
using MediPoint.Application.Features.Doctors.AppointmentsQuery.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.AppointmentsQuery;

public sealed record GetDoctorAppointmentsQuery(Guid DoctorId, DateOnly? Date) : IRequest<List<AppointmentResponse>>;
