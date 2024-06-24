using System;
using System.Collections.Generic;

namespace AutomaticLaborCostSystem.DataBase;

public partial class ActiveWindow
{
    public int Id { get; set; }

    public int IdWorkingHours { get; set; }

    public string Title { get; set; } = null!;

    public TimeSpan? ActualTime { get; set; }

    public virtual WorkingHour IdWorkingHoursNavigation { get; set; } = null!;
}
