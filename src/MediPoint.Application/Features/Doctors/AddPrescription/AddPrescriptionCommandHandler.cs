using Mapster;
using MediatR;
using MediPoint.Application.Common;
using MediPoint.Application.Common.Exceptions;
using MediPoint.Application.Features.Doctors.AddPrescription.DTOs;
using MediPoint.Domain.Entities.Prescriptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using MediPoint.Domain.Entities.Prescriptions.Med;
using MediPoint.Domain.Entities.Prescriptions.LabRes;
namespace MediPoint.Application.Features.Doctors.AddPrescription;

public class AddPrescriptionCommandHandler(IAppDbContext dbContext,IMedicineService medicineService,ILabResultService labResultService) : IRequestHandler<AddPrescriptionCommand, PrescriptionResponse>
{
    public async Task<PrescriptionResponse> Handle(AddPrescriptionCommand request, CancellationToken cancellationToken)
    {
        var app = await dbContext.Appointments.FirstOrDefaultAsync(app=>app.Id.Equals(request.PrescriptionRequest.AppointmentId));
        if (app is null)
        {
            throw new NotFoundException("Appointment",request.PrescriptionRequest.AppointmentId.ToString());
        }

        var req = request.PrescriptionRequest;

        Prescription prs = request.PrescriptionRequest.Adapt<Prescription>();
        prs.AppointmentId = app.Id;
        prs.PatientId = app.PatientId;
        prs.DoctorId = app.DoctorId;

        await dbContext.Prescriptions.AddAsync(prs);
        await dbContext.SaveChangesAsync(cancellationToken);

        if(!string.IsNullOrEmpty(req.MedicineName))
            await medicineService.CreateAsync(new Medicine { Dosage = req.Dosage,DurationDays=req.DurationDays,
                Instructions=req.Instructions,
                Frequency=req.Frequency,
                Name=req.MedicineName,
                PatientId=app.PatientId,
                PrescriptionId=prs.Id
            });


        if (!string.IsNullOrEmpty(req.TestName))
            await labResultService.CreateAsync(new LabResult { Result=req.Result,TestName=req.TestName,
                Unit=req.Unit,ReferenceRange=req.ReferenceRange,PatientId=app.PatientId,PrescriptionId=prs.Id});


        return prs.Adapt<PrescriptionResponse>();

    }
}
