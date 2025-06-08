using FoodWeb.Data;
using FoodWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FoodWeb.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

       // GET: /Users
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.Include(u => u.Role).ToListAsync();
            ViewBag.Roles = await _context.Roles.ToListAsync();
            return View(users);
        }

        // GET: /Users/GetUser/5
        [HttpGet]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return Json(new { success = false, message = "Người dùng không tồn tại" });
            
            return Json(new
            {
                id = user.Id,
                username = user.Username,
                email = user.Email,
                fullname = user.Fullname,        
                roleId = user.RoleId,
                isLocked = user.IsLocked
            });

        }

        // POST: /Users/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Users user, [FromForm] string Password)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(Password))
                return Json(new { success = false, message = "Vui lòng điền đầy đủ thông tin" });

            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                return Json(new { success = false, message = "Tên đăng nhập đã tồn tại" });

            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return Json(new { success = false, message = "Email đã tồn tại" });
            user.Fullname = user.Fullname; 
            user.PasswordHash = HashPassword(Password);
            user.CreatedAt = DateTime.Now;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST: /Users/Edit
        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] Users user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
                return Json(new { success = false, message = "Người dùng không tồn tại" });

            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.RoleId = user.RoleId;
            existingUser.IsLocked = user.IsLocked;
            existingUser.Fullname = user.Fullname;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        // POST: /Users/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return Json(new { success = false, message = "Người dùng không tồn tại" });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
