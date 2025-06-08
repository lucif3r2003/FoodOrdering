using System;
using System.Collections.Generic;

namespace FoodWeb.Models;

public partial class Reviews
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int MenuItemId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }
    public string? Reply { get; set; }
    public DateTime? ReplyAt { get; set; }

    public virtual Users Customer { get; set; } = null!;

    public virtual MenuItems MenuItem { get; set; } = null!;
}
