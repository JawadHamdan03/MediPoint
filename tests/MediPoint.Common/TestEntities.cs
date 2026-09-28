using MediPoint.Domain.Entities.User;

namespace MediPoint.Common;

public static class TestEntities
{
    public static Doctor NewDoctor(
        string email = "dr.house@example.com",
        string firstName = "Greg",
        string lastName = "House",
        string password = "Password1!",
        string specialty = "Diagnostics",
        string licenseNumber = "LIC-001") => new()
    {
        Email = email,
        FirstName = firstName,
        LastName = lastName,
        PhoneNumber = "+15551234567",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        Specialty = specialty,
        LicenseNumber = licenseNumber,
    };

    public static Patient NewPatient(
        string email = "jane.doe@example.com",
        string firstName = "Jane",
        string lastName = "Doe",
        string password = "Password1!") => new()
    {
        Email = email,
        FirstName = firstName,
        LastName = lastName,
        PhoneNumber = "+15551234567",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
    };

    public static Admin NewAdmin(
        string email = "admin@example.com",
        string firstName = "Ada",
        string lastName = "Admin",
        string password = "Password1!") => new()
    {
        Email = email,
        FirstName = firstName,
        LastName = lastName,
        PhoneNumber = "+15551234567",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
    };
}
