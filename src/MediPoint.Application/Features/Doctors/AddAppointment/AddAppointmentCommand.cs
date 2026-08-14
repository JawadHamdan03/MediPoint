using MediatR;
using MediPoint.Application.Features.Doctors.AddAppointment.DTOs;
using MediPoint.Domain.Entities.Apointments;
using System;
using System.Collections.Generic;
using System.Text;


namespace MediPoint.Application.Features.Doctors.AddAppointment;

public sealed record AddAppointmentCommand(ApponitmentDTO appointment) : IRequest<ApponitmentDTO>;