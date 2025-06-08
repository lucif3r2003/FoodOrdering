using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoodWeb.Data;
using System.Security.Claims;

namespace FoodWeb.ViewComponents
{
    public class CartItemCountViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CartItemCountViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int count = 0;

            if (User.Identity?.IsAuthenticated == true)
            {
                var customerIdStr = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(customerIdStr, out int customerId))
                {
                    count = await _context.Cart
                        .Where(c => c.CustomerId == customerId)
                        .SumAsync(c => c.Quantity ?? 0);
                }
            }

            return View(count);
        }
    }
}