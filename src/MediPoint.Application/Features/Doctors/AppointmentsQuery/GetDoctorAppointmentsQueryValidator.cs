using FluentValidation;

namespace MediPoint.Application.Features.Doctors.AppointmentsQuery;

public class GetDoctorAppointmentsQueryValidator : AbstractValidator<GetDoctorAppointmentsQuery>
{
    public GetDoctorAppointmentsQueryValidator()
    {
        RuleFor(x => x.DoctorId).NotEmpty();
    }
}
