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
using MediPoint.Domain.Entities.User;
using Microsoft.Extensions.Caching.Memory;

namespace MediPoint.Application.Features.Patients.FindDoctors;

public class FindDoctorsQueryHandler(IAppDbContext dbContext,ILogger<FindDoctorsQueryHandler> logger,IMemoryCache memoryCache) 
    : IRequestHandler<FindDoctorsQuery, List<DoctorResponse>>
{
    public async Task<List<DoctorResponse>> Handle(FindDoctorsQuery request, CancellationToken cancellationToken)
    {
        async Task AttachRatingsAsync(List<DoctorResponse> list)
        {
            var doctorIds = list.Select(d => d.Id).ToList();
            var ratings = await dbContext.Reviews.AsNoTracking()
                .Where(r => doctorIds.Contains(r.DoctorId))
                .GroupBy(r => r.DoctorId)
                .Select(g => new { DoctorId = g.Key, Average = g.Average(r => r.Rating), Count = g.Count() })
                .ToListAsync(cancellationToken);

            foreach (var doc in list)
            {
                var rating = ratings.FirstOrDefault(r => r.DoctorId == doc.Id);
                if (rating is not null)
                {
                    doc.AverageRating = Math.Round(rating.Average, 1);
                    doc.ReviewCount = rating.Count;
                }
            }
        }

        string doctorsBySpecialityCacheKey = $"doctors-spec-{request.speciality}";
        List<Doctor>? doctors;
        if (memoryCache.TryGetValue(doctorsBySpecialityCacheKey,out doctors ))
        {
            logger.LogInformation("Docotors with {Speciality} found in cache ",request.speciality);
            List<DoctorResponse> cacheRes = [];
            foreach (var doc in doctors)
            {
                var aps = doc.Appointments.Adapt<List<AppointmentDTO>>();
                var adaptedDoc=doc.Adapt<DoctorResponse>();
                adaptedDoc.AppointmentDTOs=aps;
                cacheRes.Add(adaptedDoc);
            }

            await AttachRatingsAsync(cacheRes);
            return cacheRes;
        }

       
        
        doctors = await dbContext.Doctors.AsNoTracking().Include(d=>d.Appointments).Where(d=>d.Specialty.Contains(request.speciality) && d.IsAvailable).ToListAsync();
        if (doctors is null)
        {
            logger.LogInformation("No doctors were found");
            throw new NotFoundException("Doctor",request.speciality);
        }
        
        memoryCache.Set(doctorsBySpecialityCacheKey, doctors, new MemoryCacheEntryOptions()
        {
            Size = 1,
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });
        
        List<DoctorResponse> res = [];
        foreach (var doc in doctors)
        {
            var aps = doc.Appointments.Adapt<List<AppointmentDTO>>();
            var adaptedDoc=doc.Adapt<DoctorResponse>();
            adaptedDoc.AppointmentDTOs=aps;
            res.Add(adaptedDoc);
        }

        await AttachRatingsAsync(res);
        logger.LogInformation("Doctors with {Speciality} Speciality found",request.speciality);
        return res;
    }
}
