using FoodWeb.Models;
using System.Collections.Generic;

namespace FoodWeb.ViewModels
{
    public class DetailsViewModel
    {
        public MenuItems MenuItem { get; set; } = null!;
        public double AverageRating { get; set; }
        public List<Reviews> Reviews { get; set; } = new List<Reviews>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}