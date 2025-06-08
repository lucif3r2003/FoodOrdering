using Microsoft.AspNetCore.Mvc;

namespace FoodWeb.Controllers
{
    public class RestaurantDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}