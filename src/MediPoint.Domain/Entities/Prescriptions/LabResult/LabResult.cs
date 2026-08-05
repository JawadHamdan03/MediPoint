using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Entities.Prescriptions.LabRes;

public class LabResult
{
    public string TestName { get; set; } = null!;

    public string Result { get; set; } = null!;

    public string Unit { get; set; } = "";

    public string ReferenceRange { get; set; } = "";
}
