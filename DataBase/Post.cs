using System;
using System.Collections.Generic;

namespace AutomaticLaborCostSystem.DataBase;

public partial class Post
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
