using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Application.Features.CreateLabResult.DTOs;

public sealed class LabResultRequest
{
    public string TestName { get; set; } = null!;

    public string Result { get; set; } = null!;

    public string Unit { get; set; } = "";

    public string ReferenceRange { get; set; } = "";
}

