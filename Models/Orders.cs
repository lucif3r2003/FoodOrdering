using System;
using System.Collections.Generic;

namespace FoodWeb.Models;

public partial class Orders
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public DateTime? OrderDate { get; set; }

    public string Status { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public virtual Users Customer { get; set; } = null!;

    public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
}
