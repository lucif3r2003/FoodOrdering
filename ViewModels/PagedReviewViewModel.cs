using System.Collections.Generic;

namespace FoodWeb.ViewModels
{
    public class PagedReviewViewModel
    {
        public List<ReviewViewModel> Reviews { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Search { get; set; } = "";
    }
}