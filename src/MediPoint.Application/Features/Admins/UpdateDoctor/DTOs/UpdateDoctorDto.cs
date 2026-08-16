using MediPoint.Domain.Entities.User.Shared.Enums;

namespace MediPoint.Application.Features.Admins.UpdateDoctor.DTOs;

public class UpdateDoctorDto
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public string Specialty { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public int YearsOfExperience { get; set; }

    public decimal ConsultationFee { get; set; }

    public string Biography { get; set; } = "";

    public bool IsAvailable { get; set; } = true;
}
