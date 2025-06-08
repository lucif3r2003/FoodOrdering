using FoodWeb.Data;
using FoodWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using FoodWeb.Models.ViewModels;

namespace FoodWeb.Controllers
{
    public class FoodManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FoodManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.MenuItems.ToListAsync();
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWithImage([FromForm] MenuItemWithImageModel model)
        {
            var item = new MenuItems
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                CreatedAt = DateTime.Now
            };

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "foodImages", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                item.ImageUrl = $"./foodImages/{fileName}";
            }

            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> EditWithImage([FromForm] MenuItemEditWithImageModel model)
        {
            var existing = await _context.MenuItems.FindAsync(model.Id);
            if (existing == null) return NotFound();

            existing.Name = model.Name;
            existing.Description = model.Description;
            existing.Price = model.Price;
            existing.UpdatedAt = DateTime.Now;

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "foodImages", fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                existing.ImageUrl = $"./foodImages/{fileName}";
            }

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // Giữ nguyên Delete (không đổi)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null) return NotFound();

            _context.MenuItems.Remove(item);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
