using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FoodWeb.Models;
using Microsoft.AspNetCore.Authorization;

namespace FoodWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Kiểm tra user đã đăng nhập chưa
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Lấy role từ claim
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (!string.IsNullOrEmpty(role))
                {
                    // Redirect theo role
                    return role switch
                    {
                        "Admin" => RedirectToAction("Index", "AdminDashboard"),
                        "Restaurant" => RedirectToAction("Index", "RestaurantDashboard"),
                        "Customer" => RedirectToAction("Index", "FoodMenu"),
                        _ => View()
                    };
                }
            }

            // Nếu chưa đăng nhập thì hiện trang Home bình thường
            ViewBag.Message = "Welcome you to Food Ordering System";
            return View();
        }
    }
}