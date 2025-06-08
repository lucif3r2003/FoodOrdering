using FoodWeb.Data;
using FoodWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FoodWeb.ViewModels;

namespace FoodWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string role)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                ModelState.AddModelError("", "Please fill all fields");
                return View();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.Role.Name == role);

            if (user == null || user.IsLocked)
            {
                ModelState.AddModelError("", "Invalid username or role, or user is locked");
                return View();
            }

            if (!VerifyPasswordHash(password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Incorrect password");
                return View();
            }

            // Create claims for authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("Fullname", user.Fullname),
            };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("MyCookieAuth", principal, new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddHours(1)
            });

            // Redirect user to the appropriate dashboard based on role
            return user.Role.Name switch
            {
                "Admin" => RedirectToAction("Index", "AdminDashboard"),
                "Restaurant" => RedirectToAction("Index", "RestaurantDashboard"),
                "Customer" => RedirectToAction("Index", "FoodMenu"),
                _ => RedirectToAction("Login")
            };
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password, string role)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                ModelState.AddModelError("", "Please fill all fields");
                return View();
            }

            var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.Name == role);
            if (roleEntity == null)
            {
                ModelState.AddModelError("", "Invalid role");
                return View();
            }

            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                ModelState.AddModelError("", "Username already exists");
                return View();
            }

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError("", "Email already exists");
                return View();
            }

            var user = new Users
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password),
                RoleId = roleEntity.Id,
                IsLocked = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Register successful! Please login.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
        
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                Id = user.Id,
                Fullname = user.Fullname ?? "",
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FindAsync(model.Id);
            if (user == null) return NotFound();

            user.Fullname = model.Fullname;
            user.Email = model.Email;

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                user.PasswordHash = HashPassword(model.NewPassword);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == storedHash;
        }
    }
}
