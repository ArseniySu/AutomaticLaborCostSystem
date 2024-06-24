using System;
using System.Collections.Generic;

namespace AutomaticLaborCostSystem.DataBase;

public partial class TaskCompletion
{
    public int IdUsers { get; set; }

    public int IdTasks { get; set; }

    public DateTime DatetimeStart { get; set; }

    public DateTime DatetimeFinish { get; set; }

    public string? Comment { get; set; }

    public virtual Task IdTasksNavigation { get; set; } = null!;

    public virtual User IdUsersNavigation { get; set; } = null!;
}
