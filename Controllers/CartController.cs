using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodWeb.Data;
using FoodWeb.Models;
using FoodWeb.ViewModels;

namespace FoodWeb.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return RedirectToAction("Login", "Account");

            var cartItems = await _context.Cart
                .Where(c => c.CustomerId == userId)
                .Include(c => c.Item)
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int[] selectedCartItemIds)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var selectedItems = await _context.Cart
                .Where(c => selectedCartItemIds.Contains(c.Id) && c.CustomerId == userId)
                .Include(c => c.Item)
                .ToListAsync();

            if (!selectedItems.Any())
            {
                ModelState.AddModelError("", "No items selected.");
                return RedirectToAction(nameof(Index));
            }

            var totalAmount = selectedItems.Sum(x => (x.Item.Price * x.Quantity) ?? 0);

            var user = await _context.Users.FindAsync(userId);

            var model = new OrderConfirmationViewModel
            {
                SelectedItems = selectedItems,
                TotalAmount = totalAmount,
                SelectedCartItemIds = selectedCartItemIds,
                Address = user?.Address ?? "",
                PaymentMethod = "COD"
            };

            return View("OrderConfirmation", model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmOrder(OrderConfirmationViewModel model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId)) return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return Unauthorized();

            // Cập nhật địa chỉ nếu lần đầu nhập
            if (string.IsNullOrWhiteSpace(user.Address) && !string.IsNullOrWhiteSpace(model.Address))
            {
                user.Address = model.Address;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            var selectedItems = await _context.Cart
                .Where(c => model.SelectedCartItemIds.Contains(c.Id) && c.CustomerId == userId)
                .Include(c => c.Item)
                .ToListAsync();

            if (!selectedItems.Any()) return RedirectToAction(nameof(Index));

            decimal totalAmount = selectedItems.Sum(x => (x.Item.Price * x.Quantity) ?? 0);

            var order = new Orders
            {
                CustomerId = userId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                PaymentStatus = model.PaymentMethod == "COD" ? "Unpaid" : "Pending"
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var cartItem in selectedItems)
            {
                _context.OrderDetails.Add(new OrderDetails
                {
                    OrderId = order.Id,
                    MenuItemId = cartItem.ItemId.Value,
                    Quantity = cartItem.Quantity ?? 1,
                    TotalPrice = cartItem.Item.Price * (cartItem.Quantity ?? 1)
                });
            }

            _context.Cart.RemoveRange(selectedItems);
            await _context.SaveChangesAsync();

            if (model.PaymentMethod == "VNPAY")
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                string paymentUrl = Helpers.VnPayHelper.CreatePaymentUrl(
                    totalAmount,
                    $"THANH TOAN HOA DON {order.Id}",
                    order.Id.ToString(),
                    ipAddress);
                return Redirect(paymentUrl);
            }

            return RedirectToAction("OrderSuccess", new { orderId = order.Id });
        }

        public IActionResult OrderSuccess(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}
