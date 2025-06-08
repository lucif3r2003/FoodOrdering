using System;
using System.Collections.Generic;

namespace FoodWeb.Models;

public partial class MenuItems
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

    public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
    
}
