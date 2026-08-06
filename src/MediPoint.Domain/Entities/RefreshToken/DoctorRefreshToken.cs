using MediPoint.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Entities.RefreshToken;

public class DoctorRefreshToken : RefreshToken
{
    
    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; }
}
