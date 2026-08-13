using MediatR;
using MediPoint.Application.Features.Doctors.AddAppointment.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.AddAppointment;

public class AddAppointmentCommandHandler : IRequestHandler<AddAppointmentCommand, ApponitmentDTO>
{
    public Task<ApponitmentDTO> Handle(AddAppointmentCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
