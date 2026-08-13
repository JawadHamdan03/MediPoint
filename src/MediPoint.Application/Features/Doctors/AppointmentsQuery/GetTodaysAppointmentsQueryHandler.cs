using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Doctors.AppointmentsQuery.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.AppointmentsQuery;

public class GetTodaysAppointmentsQueryHandler(IAppDbContext dbContext,ILogger<GetTodaysAppointmentsQueryHandler>logger) : IRequestHandler<GetTodaysAppointmentsQuery, List<AppointmentResponse>>
{
    public async Task<List<AppointmentResponse>> Handle(GetTodaysAppointmentsQuery request, CancellationToken cancellationToken)
    {
        var doc = await dbContext.Doctors
            .Include(d => d.Appointments)
            .FirstOrDefaultAsync(d => d.Id.Equals(request.DoctorId), cancellationToken);

        if (doc is null)
        {
            logger.LogError("Doctor with this Id {DoctorId} was not found", request.DoctorId);
            throw new NotFoundException("Doctor", request.DoctorId.ToString());
        }

        DateTime today = DateTime.Today;
        DateTime tomorrow = today.AddDays(1);

        var apps =  dbContext.Appointments
            .Where(a => a.DoctorId == request.DoctorId && a.AppointmentDate >= today && a.AppointmentDate < tomorrow);

        Console.WriteLine(apps.ToQueryString());

        var res = apps.Adapt<List<AppointmentResponse>>();
        return res;
    }
}
