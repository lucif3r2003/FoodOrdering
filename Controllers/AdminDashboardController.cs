using Microsoft.AspNetCore.Mvc;

namespace FoodWeb.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}