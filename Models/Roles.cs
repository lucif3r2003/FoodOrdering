using System;
using System.Collections.Generic;

namespace FoodWeb.Models;

public partial class Roles
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Users> Users { get; set; } = new List<Users>();
}
