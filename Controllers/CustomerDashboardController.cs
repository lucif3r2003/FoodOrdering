using Microsoft.AspNetCore.Mvc;

namespace FoodWeb.Controllers
{
    public class CustomerDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}