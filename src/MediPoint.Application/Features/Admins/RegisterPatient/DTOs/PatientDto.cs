using MediPoint.Domain.Entities.User.Shared.Enums;

namespace MediPoint.Application.Features.Admins.RegisterPatient.DTOs;

public class PatientDto
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public string BloodType { get; set; } = "";

    public string Address { get; set; } = "";

    public string EmergencyContactName { get; set; } = "";

    public string EmergencyContactPhone { get; set; } = "";
}
