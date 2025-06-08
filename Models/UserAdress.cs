using System;
using System.Collections.Generic;

namespace FoodWeb.Models;

public partial class UserAdress
{
    public int Id { get; set; }

    public string? Username { get; set; }

    public string? AdressDetail { get; set; }
}
