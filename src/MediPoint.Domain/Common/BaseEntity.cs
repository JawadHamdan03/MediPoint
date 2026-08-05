using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Common;

public class BaseEntity 
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; }
}
