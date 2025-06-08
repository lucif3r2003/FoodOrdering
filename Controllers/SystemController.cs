using FoodWeb.Data;
using FoodWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodWeb.Controllers
{
    public class SystemController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SystemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /System/Index
        public async Task<IActionResult> Index()
        {
            // Lấy dòng đầu tiên trong bảng RestaurantInfo
            var restaurant = await _context.RestaurantInfo.FirstOrDefaultAsync();

            if (restaurant == null)
            {
                // Nếu chưa có thì tạo mới 1 object rỗng
                restaurant = new RestaurantInfo();
            }

            return View(restaurant);
        }

        // POST: /System/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RestaurantInfo model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var restaurant = await _context.RestaurantInfo.FirstOrDefaultAsync();

            if (restaurant == null)
            {
                // Thêm mới nếu chưa có
                _context.RestaurantInfo.Add(model);
            }
            else
            {
                // Cập nhật các trường
                restaurant.Name = model.Name;
                restaurant.Address = model.Address;
                restaurant.ContactPhone = model.ContactPhone;
                restaurant.ContactEmail = model.ContactEmail;

                _context.RestaurantInfo.Update(restaurant);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật thông tin nhà hàng thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}