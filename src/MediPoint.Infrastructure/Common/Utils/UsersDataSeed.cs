using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.User;
using MediPoint.Domain.Entities.User.Shared.Enums;
using MediPoint.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MediPoint.Infrastructure.Common.Utils;

public static class UsersDataSeed
{
    public static async Task SeedUsers(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetService<AppDbContext>();
        if (dbContext == null) return;

        await SeedAdmins(dbContext);
        await SeedDoctors(dbContext);
        await SeedPatients(dbContext);
        await SeedAppointments(dbContext);
    }

    private static async Task SeedAdmins(AppDbContext dbContext)
    {
        if (await dbContext.Admins.AnyAsync()) return;

        var admins = new List<Admin>
        {
            new Admin
            {
                FirstName = "John",
                LastName = "Administrator",
                Email = "admin@medipoint.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                PhoneNumber = "+1234567890",
                DateOfBirth = new DateOnly(1985, 5, 15),
                Gender = Gender.Male,
            },
            new Admin
            {
                FirstName = "Jawad",
                LastName = "Administrator",
                Email = "jawadhamdan003@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                PhoneNumber = "0592163158",
                DateOfBirth = new DateOnly(1985, 5, 15),
                Gender = Gender.Male,
            },
            new Admin
            {
                FirstName = "Sarah",
                LastName = "Manager",
                Email = "sarah.manager@medipoint.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                PhoneNumber = "+1234567891",
                DateOfBirth = new DateOnly(1990, 8, 22),
                Gender = Gender.Female,
            },
            new Admin
            {
                FirstName = "Omar",
                LastName = "Khalil",
                Email = "omar.khalil@medipoint.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                PhoneNumber = "+1234567892",
                DateOfBirth = new DateOnly(1988, 1, 9),
                Gender = Gender.Male,
            }
        };

        await dbContext.Admins.AddRangeAsync(admins);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedDoctors(AppDbContext dbContext)
    {
        if (await dbContext.Doctors.AnyAsync()) return;

        var doctors = new List<Doctor>
        {
            new Doctor
            {
                FirstName = "Michael",
                LastName = "Smith",
                Email = "dr.smith@medipoint.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                PhoneNumber = "+1234567901",
                DateOfBirth = new DateOnly(1980, 3, 10),
                Gender = Gender.Male,
                Specialty = "Cardiology",
                LicenseNumber = "LIC-CARD-001",
                YearsOfExperience = 15,
                ConsultationFee = 150.00m,
                Biography = "Board-certified cardiologist specializing in heart disease prevention, echocardiography, and hypertension management.",
            },
            new Doctor
            {
                FirstName = "Emily",
                LastName = "Johnson",
                Email = "dr.johnson@medipoint.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                PhoneNumber = "+1234567902",
                DateOfBirth = new DateOnly(1985, 7, 18),
                Gender = Gender.Female,
                Specialty = "Pediatrics",
                LicenseNumber = "LIC-PEDI-002",
                YearsOfExperience = 10,
                ConsultationFee = 120.00m,
                Biography = "Pediatrician focused on childhood immunizations, growth monitoring, and developmental screening.",
            },
            new Doctor
            {
                FirstName = "David",
                LastName = "Williams",
                Email = "dr.williams@medipoint.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                PhoneNumber = "+1234567903",
                DateOfBirth = new DateOnly(1978, 11, 5),
                Gender = Gender.Male,
                Specialty = "Orthopedics",
                LicenseNumber = "LIC-ORTH-003",
                YearsOfExperience = 18,
                ConsultationFee = 180.00m,
                Biography = "Orthopedic surgeon specializing in joint replacement, sports injuries, and post-operative rehabilitation.",
            },
            new Doctor
            {
                FirstName = "Lisa",
                LastName = "Brown",
                Email = "dr.brown@medipoint.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                PhoneNumber = "+1234567904",
                DateOfBirth = new DateOnly(1982, 9, 25),
                Gender = Gender.Female,
                Specialty = "Dermatology",
                LicenseNumber = "LIC-DERM-004",
                YearsOfExperience = 12,
                ConsultationFee = 130.00m,
                Biography = "Dermatologist specializing in acne, eczema, skin cancer screening, and cosmetic procedures.",
            },
            new Doctor
            {
                FirstName = "James",
                LastName = "Davis",
                Email = "dr.davis@medipoint.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                PhoneNumber = "+1234567905",
                DateOfBirth = new DateOnly(1975, 4, 12),
                Gender = Gender.Male,
                Specialty = "Neurology",
                LicenseNumber = "LIC-NEUR-005",
                YearsOfExperience = 20,
                ConsultationFee = 200.00m,
                Biography = "Neurologist with expertise in migraines, epilepsy, and stroke rehabilitation.",
            },
            new Doctor
            {
                FirstName = "Nadia",
                LastName = "Hassan",
                Email = "dr.hassan@medipoint.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                PhoneNumber = "+1234567906",
                DateOfBirth = new DateOnly(1987, 2, 3),
                Gender = Gender.Female,
                Specialty = "General Practice",
                LicenseNumber = "LIC-GENP-006",
                YearsOfExperience = 8,
                ConsultationFee = 90.00m,
                Biography = "Family physician providing routine checkups, preventive care, and chronic disease management.",
            },
            new Doctor
            {
                FirstName = "Robert",
                LastName = "Kim",
                Email = "dr.kim@medipoint.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                PhoneNumber = "+1234567907",
                DateOfBirth = new DateOnly(1979, 6, 29),
                Gender = Gender.Male,
                Specialty = "Psychiatry",
                LicenseNumber = "LIC-PSYC-007",
                YearsOfExperience = 16,
                ConsultationFee = 160.00m,
                Biography = "Psychiatrist specializing in anxiety, depression, and cognitive behavioral therapy.",
            },
            new Doctor
            {
                FirstName = "Sophia",
                LastName = "Rossi",
                Email = "dr.rossi@medipoint.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                PhoneNumber = "+1234567908",
                DateOfBirth = new DateOnly(1983, 12, 17),
                Gender = Gender.Female,
                Specialty = "Gynecology",
                LicenseNumber = "LIC-GYNE-008",
                YearsOfExperience = 13,
                ConsultationFee = 140.00m,
                Biography = "Gynecologist specializing in reproductive health, prenatal care, and minimally invasive surgery.",
            },
            new Doctor
            {
                FirstName = "Ahmed",
                LastName = "Farouk",
                Email = "dr.farouk@medipoint.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                PhoneNumber = "+1234567909",
                DateOfBirth = new DateOnly(1976, 10, 21),
                Gender = Gender.Male,
                Specialty = "ENT",
                LicenseNumber = "LIC-ENT-009",
                YearsOfExperience = 19,
                ConsultationFee = 145.00m,
                Biography = "Ear, nose, and throat specialist treating sinus disorders, hearing loss, and chronic ear infections.",
            },
            new Doctor
            {
                FirstName = "Grace",
                LastName = "Turner",
                Email = "dr.turner@medipoint.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                PhoneNumber = "+1234567910",
                DateOfBirth = new DateOnly(1991, 3, 8),
                Gender = Gender.Female,
                Specialty = "Ophthalmology",
                LicenseNumber = "LIC-OPHT-010",
                YearsOfExperience = 6,
                ConsultationFee = 110.00m,
                Biography = "Ophthalmologist providing vision correction, cataract evaluation, and glaucoma screening.",
            }
        };

        await dbContext.Doctors.AddRangeAsync(doctors);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedPatients(AppDbContext dbContext)
    {
        if (await dbContext.Patients.AnyAsync()) return;

        var patients = new List<Patient>
        {
            new Patient
            {
                FirstName = "Alice",
                LastName = "Wilson",
                Email = "alice.wilson@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                PhoneNumber = "+1234568001",
                DateOfBirth = new DateOnly(1995, 6, 20),
                Gender = Gender.Female,
                BloodType = "A+",
                Address = "123 Main Street, New York, NY 10001",
                EmergencyContactName = "Robert Wilson",
                EmergencyContactPhone = "+1234568002"
            },
            new Patient
            {
                FirstName = "Robert",
                LastName = "Martinez",
                Email = "robert.martinez@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                PhoneNumber = "+1234568003",
                DateOfBirth = new DateOnly(1988, 2, 14),
                Gender = Gender.Male,
                BloodType = "O+",
                Address = "456 Oak Avenue, Los Angeles, CA 90001",
                EmergencyContactName = "Maria Martinez",
                EmergencyContactPhone = "+1234568004"
            },
            new Patient
            {
                FirstName = "Jennifer",
                LastName = "Garcia",
                Email = "jennifer.garcia@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                PhoneNumber = "+1234568005",
                DateOfBirth = new DateOnly(1992, 10, 8),
                Gender = Gender.Female,
                BloodType = "B+",
                Address = "789 Pine Road, Chicago, IL 60601",
                EmergencyContactName = "Carlos Garcia",
                EmergencyContactPhone = "+1234568006"
            },
            new Patient
            {
                FirstName = "William",
                LastName = "Anderson",
                Email = "william.anderson@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                PhoneNumber = "+1234568007",
                DateOfBirth = new DateOnly(1970, 12, 30),
                Gender = Gender.Male,
                BloodType = "AB+",
                Address = "321 Elm Street, Houston, TX 77001",
                EmergencyContactName = "Linda Anderson",
                EmergencyContactPhone = "+1234568008"
            },
            new Patient
            {
                FirstName = "Emma",
                LastName = "Thomas",
                Email = "emma.thomas@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                PhoneNumber = "+1234568009",
                DateOfBirth = new DateOnly(2000, 4, 16),
                Gender = Gender.Female,
                BloodType = "O-",
                Address = "654 Maple Drive, Phoenix, AZ 85001",
                EmergencyContactName = "John Thomas",
                EmergencyContactPhone = "+1234568010"
            },
            new Patient
            {
                FirstName = "Daniel",
                LastName = "Lee",
                Email = "daniel.lee@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                PhoneNumber = "+1234568011",
                DateOfBirth = new DateOnly(1983, 8, 7),
                Gender = Gender.Male,
                BloodType = "A-",
                Address = "987 Cedar Lane, Philadelphia, PA 19101",
                EmergencyContactName = "Susan Lee",
                EmergencyContactPhone = "+1234568012"
            },
            new Patient
            {
                FirstName = "Olivia",
                LastName = "Clark",
                Email = "olivia.clark@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                PhoneNumber = "+1234568013",
                DateOfBirth = new DateOnly(1998, 1, 25),
                Gender = Gender.Female,
                BloodType = "B-",
                Address = "159 Birch Boulevard, Seattle, WA 98101",
                EmergencyContactName = "Mark Clark",
                EmergencyContactPhone = "+1234568014"
            },
            new Patient
            {
                FirstName = "Yusuf",
                LastName = "Rahman",
                Email = "yusuf.rahman@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                PhoneNumber = "+1234568015",
                DateOfBirth = new DateOnly(1965, 5, 3),
                Gender = Gender.Male,
                BloodType = "AB-",
                Address = "753 Spruce Court, Miami, FL 33101",
                EmergencyContactName = "Amina Rahman",
                EmergencyContactPhone = "+1234568016"
            },
            new Patient
            {
                FirstName = "Sophia",
                LastName = "Nguyen",
                Email = "sophia.nguyen@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                PhoneNumber = "+1234568017",
                DateOfBirth = new DateOnly(2003, 9, 11),
                Gender = Gender.Female,
                BloodType = "O+",
                Address = "246 Willow Way, Austin, TX 73301",
                EmergencyContactName = "Minh Nguyen",
                EmergencyContactPhone = "+1234568018"
            },
            new Patient
            {
                FirstName = "George",
                LastName = "Papadopoulos",
                Email = "george.papadopoulos@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                PhoneNumber = "+1234568019",
                DateOfBirth = new DateOnly(1958, 7, 19),
                Gender = Gender.Male,
                BloodType = "A+",
                Address = "864 Chestnut Street, Boston, MA 02101",
                EmergencyContactName = "Eleni Papadopoulos",
                EmergencyContactPhone = "+1234568020"
            }
        };

        await dbContext.Patients.AddRangeAsync(patients);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedAppointments(AppDbContext dbContext)
    {
        if (await dbContext.Appointments.AnyAsync()) return;

        var doctorsByEmail = await dbContext.Doctors.AsNoTracking().ToDictionaryAsync(d => d.Email);
        var patientsByEmail = await dbContext.Patients.AsNoTracking().ToDictionaryAsync(p => p.Email);

        if (doctorsByEmail.Count == 0 || patientsByEmail.Count == 0) return;

        var appointments = new List<Appointment>();

        void AddConfirmed(string doctorEmail, string patientEmail, DateTime start, int minutes, string reason)
        {
            var appointment = Appointment.Create(doctorsByEmail[doctorEmail].Id, start, minutes, reason);
            appointment.Confirm(patientsByEmail[patientEmail].Id);
            appointments.Add(appointment);
        }

        void AddOpenSlot(string doctorEmail, DateTime start, int minutes, string reason)
        {
            appointments.Add(Appointment.Create(doctorsByEmail[doctorEmail].Id, start, minutes, reason));
        }

        void AddCompleted(string doctorEmail, string patientEmail, DateTime start, int minutes, string reason, string notes)
        {
            var appointment = Appointment.Create(doctorsByEmail[doctorEmail].Id, start, minutes, reason);
            appointment.Confirm(patientsByEmail[patientEmail].Id);
            appointment.Complete(notes);
            appointments.Add(appointment);
        }

        void AddCancelled(string doctorEmail, string patientEmail, DateTime start, int minutes, string reason, string cancellationReason)
        {
            var appointment = Appointment.Create(doctorsByEmail[doctorEmail].Id, start, minutes, reason);
            appointment.Confirm(patientsByEmail[patientEmail].Id);
            appointment.Cancel(cancellationReason);
            appointments.Add(appointment);
        }

        var now = DateTime.Now;

        // Upcoming confirmed appointments
        AddConfirmed("dr.smith@medipoint.com", "jennifer.garcia@email.com", now.AddHours(1), 30, "General Checkup");
        AddConfirmed("dr.johnson@medipoint.com", "robert.martinez@email.com", now.AddDays(3).AddHours(14), 30, "Child Health Checkup");
        AddConfirmed("dr.brown@medipoint.com", "william.anderson@email.com", now.AddDays(10).AddHours(9), 30, "Skin Condition Evaluation");
        AddConfirmed("dr.davis@medipoint.com", "emma.thomas@email.com", now.AddDays(2).AddHours(15), 45, "Neurological Examination");
        AddConfirmed("dr.hassan@medipoint.com", "olivia.clark@email.com", now.AddDays(1).AddHours(10), 20, "Annual Physical");
        AddConfirmed("dr.kim@medipoint.com", "yusuf.rahman@email.com", now.AddDays(4).AddHours(11), 45, "Anxiety Follow-up");
        AddConfirmed("dr.rossi@medipoint.com", "sophia.nguyen@email.com", now.AddDays(5).AddHours(13), 30, "Prenatal Checkup");
        AddConfirmed("dr.farouk@medipoint.com", "george.papadopoulos@email.com", now.AddDays(9).AddHours(16), 20, "Chronic Sinusitis Review");

        // Open (unbooked) slots
        AddOpenSlot("dr.williams@medipoint.com", now.AddDays(7).AddHours(11), 45, "Joint Pain Assessment");
        AddOpenSlot("dr.smith@medipoint.com", now.AddDays(6).AddHours(13), 30, "Cardiac Consultation");
        AddOpenSlot("dr.turner@medipoint.com", now.AddDays(12).AddHours(9), 30, "Vision Screening");

        // Completed appointments (past)
        AddCompleted("dr.johnson@medipoint.com", "alice.wilson@email.com", now.AddDays(-5).AddHours(10), 30, "Follow-up Visit", "Completed routine examination, no concerns noted.");
        AddCompleted("dr.smith@medipoint.com", "daniel.lee@email.com", now.AddDays(-12).AddHours(9), 30, "Blood Pressure Review", "Blood pressure within normal range, continue current medication.");
        AddCompleted("dr.davis@medipoint.com", "william.anderson@email.com", now.AddDays(-20).AddHours(14), 45, "Migraine Consultation", "Prescribed preventive treatment, follow-up in 6 weeks.");
        AddCompleted("dr.kim@medipoint.com", "jennifer.garcia@email.com", now.AddDays(-8).AddHours(11), 45, "Initial Psychiatric Assessment", "Diagnosed mild anxiety disorder, started therapy plan.");

        // Cancelled appointments
        AddCancelled("dr.williams@medipoint.com", "robert.martinez@email.com", now.AddDays(8).AddHours(16), 30, "Joint Surgery Consultation", "Patient requested to reschedule.");
        AddCancelled("dr.brown@medipoint.com", "olivia.clark@email.com", now.AddDays(-2).AddHours(10), 30, "Skin Allergy Test", "Doctor unavailable due to emergency.");

        await dbContext.Appointments.AddRangeAsync(appointments);
        await dbContext.SaveChangesAsync();
    }
}
