using System;
using System.Collections.Generic;

namespace FoodWeb.Models;

public partial class Cart
{
    public int Id { get; set; }

    public int? CustomerId { get; set; }

    public int? ItemId { get; set; }

    public int? Quantity { get; set; }
    public MenuItems Item { get; set; }
}
