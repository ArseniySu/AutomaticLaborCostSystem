using System;
using System.Collections.Generic;

namespace AutomaticLaborCostSystem.DataBase;

public partial class ProductionCalendar
{
    public int Id { get; set; }
    public DateTime Date_ { get; set; }

    public string NormHours { get; set; } = null!;

    public string? Comment { get; set; }
}
