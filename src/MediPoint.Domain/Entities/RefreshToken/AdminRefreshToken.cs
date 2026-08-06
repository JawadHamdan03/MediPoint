using MediPoint.Domain.Common;
using MediPoint.Domain.Entities.User;
using MediPoint.Domain.Entities.User.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Entities.RefreshToken;

public class AdminRefreshToken : RefreshToken
{
    
    public Guid AdminId { get; set; }
    public Admin Admin { get; set; }
}
