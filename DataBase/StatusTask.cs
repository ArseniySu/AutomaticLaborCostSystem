using System;
using System.Collections.Generic;

namespace AutomaticLaborCostSystem.DataBase;

public partial class StatusTask
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
