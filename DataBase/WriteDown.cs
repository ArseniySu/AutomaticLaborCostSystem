using System;
using System.Collections.Generic;

namespace AutomaticLaborCostSystem.DataBase;

public partial class WriteDown
{
    public int Id { get; set; }

    public int IdWorkingHours { get; set; }

    public string Reason { get; set; } = null!;

    public TimeSpan? ActualTime { get; set; }

    public string? Comment { get; set; }

    public virtual WorkingHour IdWorkingHoursNavigation { get; set; } = null!;
}
