using MediPoint.Domain.Common;
using MediPoint.Domain.Entities.User.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Entities.User.Shared;

public class BaseUser : BaseEntity
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public Gender Gender { get; set; }
    public bool IsActive { get; set; } = true;

}
