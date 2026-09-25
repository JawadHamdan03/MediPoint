using MediPoint.Domain.Entities.User.Shared.Enums;

namespace MediPoint.Application.Features.Doctors.SignUp.DTOs;

public class DoctorSignUpDto
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public string Specialty { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public int YearsOfExperience { get; set; }

    public decimal ConsultationFee { get; set; }

    public string Biography { get; set; } = "";
}
