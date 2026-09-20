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
                var confirmedCheckup = Appointment.Create(doctors[0].Id, DateTime.Now.AddHours(1), 30, "General Checkup");
                confirmedCheckup.Confirm(patients[2].Id);

                var confirmedPediatric = Appointment.Create(doctors[1].Id, DateTime.Now.AddDays(3).AddHours(14), 30, "Child Health Checkup");
                confirmedPediatric.Confirm(patients[1].Id);

                var openJointSlot = Appointment.Create(doctors[2].Id, DateTime.Now.AddDays(7).AddHours(11), 45, "Joint Pain Assessment");

                var confirmedDermatology = Appointment.Create(doctors[3].Id, DateTime.Now.AddDays(10).AddHours(9), 30, "Skin Condition Evaluation");
                confirmedDermatology.Confirm(patients[3].Id);

                var confirmedNeurology = Appointment.Create(doctors[4].Id, DateTime.Now.AddDays(2).AddHours(15), 45, "Neurological Examination");
                confirmedNeurology.Confirm(patients[4].Id);

                var openCardiacSlot = Appointment.Create(doctors[0].Id, DateTime.Now.AddDays(6).AddHours(13), 30, "Cardiac Consultation");

                var completedFollowUp = Appointment.Create(doctors[1].Id, DateTime.Now.AddDays(-5).AddHours(10), 30, "Follow-up Visit");
                completedFollowUp.Confirm(patients[0].Id);
                completedFollowUp.Complete("Completed routine examination.");

                var cancelledSurgeryConsult = Appointment.Create(doctors[2].Id, DateTime.Now.AddDays(8).AddHours(16), 30, "Joint Surgery Consultation");
                cancelledSurgeryConsult.Confirm(patients[1].Id);
                cancelledSurgeryConsult.Cancel("Patient requested to reschedule.");

                var appointments = new List<Appointment>
                {
                    confirmedCheckup,
                    confirmedPediatric,
                    openJointSlot,
                    confirmedDermatology,
                    confirmedNeurology,
                    openCardiacSlot,
                    completedFollowUp,
                    cancelledSurgeryConsult
                };

                await dbContext.Appointments.AddRangeAsync(appointments);
            }
        }

        await dbContext.SaveChangesAsync();
    }
}
