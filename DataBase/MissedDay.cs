using System;
using System.Collections.Generic;

namespace AutomaticLaborCostSystem.DataBase;

public partial class MissedDay
{
    public int Id { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime DatetimeStart { get; set; }

    public DateTime DatetimeFinish { get; set; }

    public string? Comment { get; set; }

    public int IdUsers { get; set; }

    public virtual User IdUsersNavigation { get; set; } = null!;
}
