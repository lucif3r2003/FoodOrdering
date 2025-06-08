using System;
using System.Collections.Generic;

namespace FoodWeb.Models;

public partial class Users
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    public bool IsLocked { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Fullname { get; set; }
    public string? Address { get; set; }
    public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();

    public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();

    public virtual Roles Role { get; set; } = null!;
    
}
