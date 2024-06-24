using System;
using System.Collections.Generic;

namespace AutomaticLaborCostSystem.DataBase;

public partial class User
{
    public int Id { get; set; }

    public string Surname { get; set; } = null!;

    public string SecondName { get; set; } = null!;

    public string? MiddlName { get; set; }

    public int IdRole { get; set; }

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int IdDepartment { get; set; }

    public string Logins { get; set; } = null!;

    public string Passwords { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int IdPost { get; set; }

    public virtual Department IdDepartmentNavigation { get; set; } = null!;

    public virtual Post IdPostNavigation { get; set; } = null!;

    public virtual Role IdRoleNavigation { get; set; } = null!;

    public virtual ICollection<MissedDay> MissedDays { get; set; } = new List<MissedDay>();

    public virtual ICollection<TaskCompletion> TaskCompletions { get; set; } = new List<TaskCompletion>();

    public virtual ICollection<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();
}
