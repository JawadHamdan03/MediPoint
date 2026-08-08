using MediatR;
using MediPoint.Application.Features.Patients.DTOs;
using MediPoint.Domain.Entities.Apointments;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.BookAppointment;

public sealed record BookAppointmentCommand(BookAppointmentRequest Request) : IRequest<Appointment>;
