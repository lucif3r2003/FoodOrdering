using FoodWeb.Data;
using FoodWeb.Models;
using FoodWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Restaurant,Admin")]
public class RestaurantReviewsController : Controller
{
    private readonly ApplicationDbContext _context;
    private const int PageSize = 5;

    public RestaurantReviewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /RestaurantReviews
    public async Task<IActionResult> Index(string search = "", int page = 1)
    {
        var query = _context.Reviews
            .Include(r => r.Customer)
            .Include(r => r.MenuItem)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(r => r.MenuItem.Name.Contains(search));
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(r => new ReviewViewModel
            {
                Id = r.Id,
                CustomerName = r.Customer.Fullname,
                MenuItemName = r.MenuItem.Name,
                Rating = r.Rating,
                Comment = r.Comment,
                Reply = r.Reply,
                CreatedAt = r.CreatedAt,
                ReplyAt = r.ReplyAt
            })
            .ToListAsync();

        var model = new PagedReviewViewModel
        {
            Reviews = reviews,
            CurrentPage = page,
            TotalPages = totalPages,
            Search = search
        };

        return View(model);
    }

    // POST: /RestaurantReviews/Reply
    [HttpPost]
    public async Task<IActionResult> Reply(int id, string reply, string search, int page)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null) return NotFound();

        review.Reply = reply;
        review.ReplyAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { search, page });
    }
}
