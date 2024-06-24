using System;
using System.Collections.Generic;

namespace AutomaticLaborCostSystem.DataBase;

public partial class Project
{
    public int Id { get; set; }

    public DateTime DatetimeStart { get; set; }

    public DateTime PlannedEndDate { get; set; }

    public DateTime? DatetimeFinish { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
