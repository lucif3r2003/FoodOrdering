using FoodWeb.Models;

public class CheckoutViewModel
{
    public List<Cart> CartItems { get; set; }
    public decimal TotalAmount { get; set; }
}