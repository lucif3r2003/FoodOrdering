using FoodWeb.Data;
using FoodWeb.Models;
using FoodWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize(Roles = "Customer")]
public class FoodMenuController : Controller
{
    private readonly ApplicationDbContext _context;

    public FoodMenuController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetMenu()
    {
        var menu = _context.MenuItems
            .Select(item => new
            {
                item.Id,
                item.Name,
                item.Description,
                item.ImageUrl,
                item.Price,
                AverageRating = item.Reviews.Any()
                    ? item.Reviews.Average(r => r.Rating)
                    : 0
            })
            .ToList();

        return Json(menu);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int itemId)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var existing = await _context.Cart
            .FirstOrDefaultAsync(x => x.CustomerId == userId && x.ItemId == itemId);

        if (existing != null)
        {
            existing.Quantity += 1;
        }
        else
        {
            _context.Cart.Add(new Cart
            {
                CustomerId = userId,
                ItemId = itemId,
                Quantity = 1
            });
        }

        await _context.SaveChangesAsync();
        return Json(new { message = "Added to cart!" });
    }

    public async Task<IActionResult> Details(int id, int page = 1, int pageSize = 5)
    {
        var menuItem = await _context.MenuItems
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menuItem == null) return NotFound();

        var reviewsQuery = _context.Reviews
            .Where(r => r.MenuItemId == id)
            .Include(r => r.Customer)
            .OrderByDescending(r => r.CreatedAt);

        var totalReviews = await reviewsQuery.CountAsync();
        var totalPages = (int)Math.Ceiling(totalReviews / (double)pageSize);

        var reviews = await reviewsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        double avgRating = 0;
        if (totalReviews > 0)
        {
            avgRating = await _context.Reviews
                .Where(r => r.MenuItemId == id)
                .AverageAsync(r => r.Rating);
        }

        var vm = new DetailsViewModel
        {
            MenuItem = menuItem,
            AverageRating = avgRating,
            Reviews = reviews,
            CurrentPage = page,
            TotalPages = totalPages
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddReview(int menuItemId, int rating, string comment)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized();

        var review = new Reviews
        {
            CustomerId = userId,
            MenuItemId = menuItemId,
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.Now
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = menuItemId });
    }
}
