using System;
using System.Collections.Generic;

namespace FoodWeb.Models;

public partial class OrderDetails
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int MenuItemId { get; set; }

    public int Quantity { get; set; }

    public decimal TotalPrice { get; set; }

    public virtual MenuItems MenuItem { get; set; } = null!;

    public virtual Orders Order { get; set; } = null!;
}
