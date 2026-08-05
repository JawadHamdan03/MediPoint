using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Domain.Entities.Prescriptions.Med;

public class Medicine
{
    public string Name { get; set; } = null!;

    public string Dosage { get; set; } = null!;

    public string Frequency { get; set; } = null!;

    public int DurationDays { get; set; }

    public string Instructions { get; set; } = "";
}
