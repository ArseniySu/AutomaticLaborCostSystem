using System;
using System.Collections.Generic;

namespace AutomaticLaborCostSystem.DataBase;

public partial class Department
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Descriptions { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
