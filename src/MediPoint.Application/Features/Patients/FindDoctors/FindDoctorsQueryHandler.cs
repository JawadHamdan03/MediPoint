using DnsClient.Internal;
using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Patients.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Patients.FindDoctors;

public class FindDoctorsQueryHandler(IAppDbContext dbContext,ILogger<FindDoctorsQueryHandler> logger) : IRequestHandler<FindDoctorsQuery, List<DoctorResponse>>
{
    public async Task<List<DoctorResponse>> Handle(FindDoctorsQuery request, CancellationToken cancellationToken)
    {
        var doctors = await dbContext.Doctors.AsNoTracking().Include(d=>d.Appointments).Where(d=>d.Specialty.Equals(request.speciality)).ToListAsync();
        if (doctors is null)
        {
            logger.LogInformation("No doctors were found");
            throw new NotFoundException("Doctor",request.speciality);
        }
        List<DoctorResponse> res = [];
        foreach (var doc in doctors)
        {
            var aps = doc.Appointments.Adapt<List<AppointmentDTO>>();
            var adaptedDoc=doc.Adapt<DoctorResponse>();
            adaptedDoc.AppointmentDTOs=aps;
            res.Add(adaptedDoc);
        }

        logger.LogInformation("Doctors with {Speciality} Speciality found",request.speciality);
        return res;
    }
}
