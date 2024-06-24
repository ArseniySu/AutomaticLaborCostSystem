using System;
using System.Collections.Generic;

namespace AutomaticLaborCostSystem.DataBase;

public partial class Task
{
    public int Id { get; set; }

    public int IdProject { get; set; }

    public string Title { get; set; } = null!;

    public int IdStatusTasks { get; set; }

    public string Descriptions { get; set; } = null!;

    public int IdDepartment { get; set; }

    public virtual Department IdDepartmentNavigation { get; set; } = null!;

    public virtual Project IdProjectNavigation { get; set; } = null!;

    public virtual StatusTask IdStatusTasksNavigation { get; set; } = null!;

    public virtual ICollection<TaskCompletion> TaskCompletions { get; set; } = new List<TaskCompletion>();
}
