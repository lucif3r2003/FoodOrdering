using System;
using System.Collections.Generic;

namespace FoodWeb.Models;

public partial class RestaurantInfo
{
    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? ContactPhone { get; set; }

    public string? ContactEmail { get; set; }

    public int? Id { get; set; }
}
