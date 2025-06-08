using FoodWeb.Models;
using System.Collections.Generic;

namespace FoodWeb.ViewModels
{
    public class OrderConfirmationViewModel
    {
        public List<Cart> SelectedItems { get; set; }
        public decimal TotalAmount { get; set; }
        public int[] SelectedCartItemIds { get; set; }

        public string? Address { get; set; }
        public string PaymentMethod { get; set; }
    }
}