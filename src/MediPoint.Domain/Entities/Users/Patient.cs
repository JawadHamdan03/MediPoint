using MediPoint.Domain.Entities.Apointments;
using MediPoint.Domain.Entities.User.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Entities.User;

public class Patient : BaseUser
{
    public string BloodType { get; set; } = "";

    public string Address { get; set; } = "";

    public string EmergencyContactName { get; set; } = "";

    public string EmergencyContactPhone { get; set; } = "";


    public List<Appointment> Appointments { get; set; }= new List<Appointment>();
}
