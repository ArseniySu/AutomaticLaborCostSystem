using System;
using System.Collections.Generic;

namespace AutomaticLaborCostSystem.DataBase;

public partial class WorkingHour
{
    public int Id { get; set; }

    public int IdUsers { get; set; }

    public DateTime DatetimeStart { get; set; }

    public DateTime? DatetimeFinish { get; set; }

    public TimeSpan? ActualTime { get; set; }

    public virtual ICollection<ActiveWindow> ActiveWindows { get; set; } = new List<ActiveWindow>();

    public virtual User IdUsersNavigation { get; set; } = null!;

    public virtual ICollection<WriteDown> WriteDowns { get; set; } = new List<WriteDown>();
}
