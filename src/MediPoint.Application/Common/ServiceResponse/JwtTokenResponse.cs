using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Common.ServiceResponse;

public class JwtTokenResponse
{

     public string? AccessToken { get; set; }
     public string? RefreshToken { get; set; }
     public DateTime ExpiresAt { get; set; }

     public Guid UserId { get; set; }
     public string? FirstName { get; set; }
     public string? LastName { get; set; }
     public string? Role { get; set; }
     public string? ImageUrl { get; set; }
}
