using MediPoint.Domain.Common;
using MediPoint.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Entities.RefreshToken;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }

   
}
