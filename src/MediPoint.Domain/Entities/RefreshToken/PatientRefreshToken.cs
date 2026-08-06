using MediPoint.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Entities.RefreshToken;

public class PatientRefreshToken : RefreshToken
{
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; }
}
