using FoodWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace FoodWeb.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị các đơn hàng của khách hiện tại
        public async Task<IActionResult> MyOrders()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Login", "Account");

            var orders = await _context.Orders
                .Where(o => o.CustomerId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // Chi tiết đơn hàng
        public async Task<IActionResult> Details(int id)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToAction("Login", "Account");

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id && o.CustomerId == userId);

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}