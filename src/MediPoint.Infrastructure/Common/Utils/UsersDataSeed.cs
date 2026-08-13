using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.Appointments.Enums;
using MediPoint.Domain.Entities.User;
using MediPoint.Domain.Entities.User.Shared.Enums;
using MediPoint.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Infrastructure.Common.Utils;

public static class UsersDataSeed
{

    public static async Task SeedUsers(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetService<AppDbContext>();
        if (dbContext == null) return;

        // Seed Admins
        if (!await dbContext.Admins.AnyAsync())
        {
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
                    FirstName = "Sarah",
                    LastName = "Manager",
                    Email = "sarah.manager@medipoint.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    PhoneNumber = "+1234567891",
                    DateOfBirth = new DateOnly(1990, 8, 22),
                    Gender = Gender.Female,
                    
                }
            };

            await dbContext.Admins.AddRangeAsync(admins);
            await dbContext.SaveChangesAsync();
        }

        // Seed Doctors
        if (!(await dbContext.Doctors.AnyAsync()))
        {
            var doctors = new List<Doctor>
            {
                new Doctor
                {
                    FirstName = "Michael",
                    LastName = "Smith",
                    Email = "dr.smith@medipoint.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                    PhoneNumber = "+1234567892",
                    DateOfBirth = new DateOnly(1980, 3, 10),
                    Gender = Gender.Male,
                    
                    Specialty = "Cardiology",
                    LicenseNumber = "LIC-CARD-001",
                    YearsOfExperience = 15,
                    ConsultationFee = 150.00m,
                    Biography = "Experienced cardiologist specializing in heart disease prevention and treatment.",
                    IsAvailable = true
                },
                new Doctor
                {
                    FirstName = "Emily",
                    LastName = "Johnson",
                    Email = "dr.johnson@medipoint.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                    PhoneNumber = "+1234567893",
                    DateOfBirth = new DateOnly(1985, 7, 18),
                    Gender = Gender.Female,
                  
                    Specialty = "Pediatrics",
                    LicenseNumber = "LIC-PEDI-002",
                    YearsOfExperience = 10,
                    ConsultationFee = 120.00m,
                    Biography = "Pediatrician with extensive experience in child healthcare and development.",
                    IsAvailable = true
                },
                new Doctor
                {
                    FirstName = "David",
                    LastName = "Williams",
                    Email = "dr.williams@medipoint.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                    PhoneNumber = "+1234567894",
                    DateOfBirth = new DateOnly(1978, 11, 5),
                    Gender = Gender.Male,
                   
                    Specialty = "Orthopedics",
                    LicenseNumber = "LIC-ORTH-003",
                    YearsOfExperience = 18,
                    ConsultationFee = 180.00m,
                    Biography = "Orthopedic surgeon specializing in joint replacement and sports injuries.",
                    IsAvailable = true
                },
                new Doctor
                {
                    FirstName = "Lisa",
                    LastName = "Brown",
                    Email = "dr.brown@medipoint.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                    PhoneNumber = "+1234567895",
                    DateOfBirth = new DateOnly(1982, 9, 25),
                    Gender = Gender.Female,
                    
                    Specialty = "Dermatology",
                    LicenseNumber = "LIC-DERM-004",
                    YearsOfExperience = 12,
                    ConsultationFee = 130.00m,
                    Biography = "Dermatologist specializing in skin conditions and cosmetic procedures.",
                    IsAvailable = true
                },
                new Doctor
                {
                    FirstName = "James",
                    LastName = "Davis",
                    Email = "dr.davis@medipoint.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Doctor@123"),
                    PhoneNumber = "+1234567896",
                    DateOfBirth = new DateOnly(1975, 4, 12),
                    Gender = Gender.Male,
                    
                    Specialty = "Neurology",
                    LicenseNumber = "LIC-NEUR-005",
                    YearsOfExperience = 20,
                    ConsultationFee = 200.00m,
                    Biography = "Neurologist with expertise in treating neurological disorders and brain conditions.",
                    IsAvailable = true
                }
            };

            await dbContext.Doctors.AddRangeAsync(doctors);
            await dbContext.SaveChangesAsync();
        }

        // Seed Patients
        if (!(await dbContext.Patients.AnyAsync()))
        {
            var patients = new List<Patient>
            {
                new Patient
                {
                    FirstName = "Alice",
                    LastName = "Wilson",
                    Email = "alice.wilson@email.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                    PhoneNumber = "+1234567897",
                    DateOfBirth = new DateOnly(1995, 6, 20),
                    Gender = Gender.Female,
                  
                    BloodType = "A+",
                    Address = "123 Main Street, New York, NY 10001",
                    EmergencyContactName = "Robert Wilson",
                    EmergencyContactPhone = "+1234567898"
                },
                new Patient
                {
                    FirstName = "Robert",
                    LastName = "Martinez",
                    Email = "robert.martinez@email.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                    PhoneNumber = "+1234567899",
                    DateOfBirth = new DateOnly(1988, 2, 14),
                    Gender = Gender.Male,
                    
                    BloodType = "O+",
                    Address = "456 Oak Avenue, Los Angeles, CA 90001",
                    EmergencyContactName = "Maria Martinez",
                    EmergencyContactPhone = "+1234567900"
                },
                new Patient
                {
                    FirstName = "Jennifer",
                    LastName = "Garcia",
                    Email = "jennifer.garcia@email.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                    PhoneNumber = "+1234567901",
                    DateOfBirth = new DateOnly(1992, 10, 8),
                    Gender = Gender.Female,
                  
                    BloodType = "B+",
                    Address = "789 Pine Road, Chicago, IL 60601",
                    EmergencyContactName = "Carlos Garcia",
                    EmergencyContactPhone = "+1234567902"
                },
                new Patient
                {
                    FirstName = "William",
                    LastName = "Anderson",
                    Email = "william.anderson@email.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                    PhoneNumber = "+1234567903",
                    DateOfBirth = new DateOnly(1970, 12, 30),
                    Gender = Gender.Male,
                    
                    BloodType = "AB+",
                    Address = "321 Elm Street, Houston, TX 77001",
                    EmergencyContactName = "Linda Anderson",
                    EmergencyContactPhone = "+1234567904"
                },
                new Patient
                {
                    FirstName = "Emma",
                    LastName = "Thomas",
                    Email = "emma.thomas@email.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                    PhoneNumber = "+1234567905",
                    DateOfBirth = new DateOnly(2000, 4, 16),
                    Gender = Gender.Female,
                   
                    BloodType = "O-",
                    Address = "654 Maple Drive, Phoenix, AZ 85001",
                    EmergencyContactName = "John Thomas",
                    EmergencyContactPhone = "+1234567906"
                },
                new Patient
                {
                    FirstName = "Daniel",
                    LastName = "Lee",
                    Email = "daniel.lee@email.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Patient@123"),
                    PhoneNumber = "+1234567907",
                    DateOfBirth = new DateOnly(1983, 8, 7),
                    Gender = Gender.Male,
                    
                    BloodType = "A-",
                    Address = "987 Cedar Lane, Philadelphia, PA 19101",
                    EmergencyContactName = "Susan Lee",
                    EmergencyContactPhone = "+1234567908"
                }
            };

            await dbContext.Patients.AddRangeAsync(patients);
            await dbContext.SaveChangesAsync();
        }

        // Seed Appointments
        if (!(await dbContext.Appointments.AnyAsync()))
        {
            // Get doctors and patients for reference
            var doctors = await dbContext.Doctors.AsNoTracking().ToListAsync();
            var patients = await dbContext.Patients.AsNoTracking().ToListAsync();

            if (doctors.Count > 0 && patients.Count > 0)
            {
                var appointments = new List<Appointment>
                {
                    new Appointment
                    {
                        PatientId = patients[2].Id,
                        DoctorId = doctors[0].Id,
                        AppointmentDate = DateTime.Now.AddHours(1),
                        Duration = 30,
                        Status = AppointmentStatus.Confirmed,
                        Reason = "General Checkup",
                        Notes = "Patient scheduled for routine heart examination."
                    },
                    new Appointment
                    {
                        PatientId = patients[1].Id,
                        DoctorId = doctors[1].Id,
                        AppointmentDate = DateTime.Now.AddDays(3).AddHours(14),
                        Duration = 30,
                        Status = AppointmentStatus.Confirmed,
                        Reason = "Child Health Checkup",
                        Notes = "Annual pediatric examination."
                    },
                    new Appointment
                    {
                        PatientId = patients[2].Id,
                        DoctorId = doctors[2].Id,
                        AppointmentDate = DateTime.Now.AddDays(7).AddHours(11),
                        Duration = 45,
                        Status = AppointmentStatus.Pending,
                        Reason = "Joint Pain Assessment",
                        Notes = "Patient reports knee pain after sports injury."
                    },
                    new Appointment
                    {
                        PatientId = patients[3].Id,
                        DoctorId = doctors[3].Id,
                        AppointmentDate = DateTime.Now.AddDays(10).AddHours(09),
                        Duration = 30,
                        Status = AppointmentStatus.Confirmed,
                        Reason = "Skin Condition Evaluation",
                        Notes = "Follow-up appointment for dermatological treatment."
                    },
                    new Appointment
                    {
                        PatientId = patients[4].Id,
                        DoctorId = doctors[4].Id,
                        AppointmentDate = DateTime.Now.AddDays(2).AddHours(15),
                        Duration = 45,
                        Status = AppointmentStatus.Confirmed,
                        Reason = "Neurological Examination",
                        Notes = "Patient experiencing occasional headaches."
                    },
                    new Appointment
                    {
                        PatientId = patients[5].Id,
                        DoctorId = doctors[0].Id,
                        AppointmentDate = DateTime.Now.AddDays(6).AddHours(13),
                        Duration = 30,
                        Status = AppointmentStatus.Pending,
                        Reason = "Cardiac Consultation",
                        Notes = "Initial consultation for heart health assessment."
                    },
                    new Appointment
                    {
                        PatientId = patients[0].Id,
                        DoctorId = doctors[1].Id,
                        AppointmentDate = DateTime.Now.AddDays(-5).AddHours(10),
                        Duration = 30,
                        Status = AppointmentStatus.Completed,
                        Reason = "Follow-up Visit",
                        Notes = "Completed routine examination."
                    },
                    new Appointment
                    {
                        PatientId = patients[1].Id,
                        DoctorId = doctors[2].Id,
                        AppointmentDate = DateTime.Now.AddDays(8).AddHours(16),
                        Duration = 30,
                        Status = AppointmentStatus.Cancelled,
                        Reason = "Joint Surgery Consultation",
                        CancellationReason = "Patient requested to reschedule."
                    }
                };

                await dbContext.Appointments.AddRangeAsync(appointments);
            }
        }

        await dbContext.SaveChangesAsync();
    }
}
