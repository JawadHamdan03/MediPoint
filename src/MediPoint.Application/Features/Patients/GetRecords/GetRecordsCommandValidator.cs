using FluentValidation;

namespace MediPoint.Application.Features.Patients.GetRecords;

public class GetRecordsCommandValidator : AbstractValidator<GetRecordsCommand>
{
    public GetRecordsCommandValidator()
    {
        RuleFor(x=>x.PatientId).NotEmpty();
    }
    
}