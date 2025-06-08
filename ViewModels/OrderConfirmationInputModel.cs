namespace FoodWeb.ViewModels
{
    public class OrderConfirmationInputModel
    {
        public int[] SelectedCartItemIds { get; set; }
        public string PaymentMethod { get; set; }  // "COD" hoặc "VNPAY"
    }
}