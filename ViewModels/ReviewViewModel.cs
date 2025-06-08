namespace FoodWeb.ViewModels;

public class ReviewViewModel
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = "";
    public string MenuItemName { get; set; } = "";
    public int Rating { get; set; }
    public string Comment { get; set; } = "";
    public string? Reply { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? ReplyAt { get; set; }
}
