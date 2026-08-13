using FluentValidation;
using MediPoint.Domain.Entities.Prescriptions.LabRes;
using MediPoint.Domain.Entities.Prescriptions.Med;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.Doctors.AddPrescription;

public class AddPrescriptionCommandValidator : AbstractValidator<AddPrescriptionCommand>
{
    public AddPrescriptionCommandValidator()
    {
        RuleFor(x => x.PrescriptionRequest.AppointmentId)
            .NotEmpty().WithMessage("Appointment ID is required.");

        RuleFor(x => x.PrescriptionRequest.Notes)
            .MaximumLength(2000).WithMessage("Notes cannot exceed 2000 characters.");

        // Individual Input Fields (if used as flat form inputs)
        RuleFor(x => x.PrescriptionRequest.MedicineName)
            .NotEmpty().WithMessage("Medicine name is required.")
            .When(x => !string.IsNullOrEmpty(x.PrescriptionRequest.Dosage)); // Example conditional: validate only if Dosage is filled

        RuleFor(x => x.PrescriptionRequest.Dosage)
            .NotEmpty().WithMessage("Dosage is required.")
            .When(x => !string.IsNullOrEmpty(x.PrescriptionRequest.MedicineName));

        RuleFor(x => x.PrescriptionRequest.DurationDays)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.PrescriptionRequest.TestName)
            .NotEmpty().WithMessage("Test name is required.")
            .When(x => !string.IsNullOrEmpty(x.PrescriptionRequest.Result));

        RuleFor(x => x.PrescriptionRequest.Result)
            .NotEmpty().WithMessage("Result is required.")
            .When(x => !string.IsNullOrEmpty(x.PrescriptionRequest.TestName));

        //// Collection Validation (applies child validators to every item in the lists)
        //RuleForEach(x => x.PrescriptionRequest.Medicines)
        //    .SetValidator(new MedicineValidator());

        //RuleForEach(x => x.PrescriptionRequest.LabResults)
        //    .SetValidator(new LabResultValidator());
    }
}




//public class MedicineValidator : AbstractValidator<Medicine>
//{
//    public MedicineValidator()
//    {
//        RuleFor(x => x.Name)
//            .NotEmpty().WithMessage("Medicine name is required.")
//            .MaximumLength(100);

//        RuleFor(x => x.Dosage)
//            .NotEmpty().WithMessage("Dosage is required.")
//            .MaximumLength(50);

//        RuleFor(x => x.Frequency)
//            .NotEmpty().WithMessage("Frequency is required.")
//            .MaximumLength(50);

//        RuleFor(x => x.DurationDays)
//            .GreaterThan(0).WithMessage("Duration must be at least 1 day.");

//        RuleFor(x => x.Instructions)
//            .MaximumLength(500);
//    }
//}

//public class LabResultValidator : AbstractValidator<LabResult>
//{
//    public LabResultValidator()
//    {
//        RuleFor(x => x.TestName)
//            .NotEmpty().WithMessage("Test name is required.")
//            .MaximumLength(100);

//        RuleFor(x => x.Result)
//            .NotEmpty().WithMessage("Test result is required.")
//            .MaximumLength(100);

//        RuleFor(x => x.Unit)
//            .MaximumLength(20);

//        RuleFor(x => x.ReferenceRange)
//            .MaximumLength(50);
//    }
//}