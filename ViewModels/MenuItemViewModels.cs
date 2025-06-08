using Microsoft.AspNetCore.Http;

namespace FoodWeb.Models.ViewModels
{
    public class MenuItemWithImageModel
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile? ImageFile { get; set; }
    }

    public class MenuItemEditWithImageModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}