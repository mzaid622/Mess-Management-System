using Mess_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace Mess_Management_System.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🟢 Show Login Page
        [HttpGet]
        public IActionResult Login_Page()
        {
            return View();
        }

        // 🟢 Handle Login Form Submission
        [HttpPost]
        public IActionResult Login_Page(string email, string password)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Email and password are required!";
                    return View();
                }

                // Hash the entered password
                string hashedPassword = HashPassword(password);

                // Find user in database
                var user = _context.Users
                    .Where(u => u.Email == email && u.PasswordHash == hashedPassword)
                    .FirstOrDefault();

                if (user != null)
                {
                    // ✅ Login successful - Save user info in session
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("UserName", user.FullName);
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserRole", user.Role);

                    // Redirect to Home
                    return RedirectToAction("Home");
                }
                else
                {
                    // ❌ Wrong credentials
                    ViewBag.Error = "Invalid email or password!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Login error: " + ex.Message;
                return View();
            }
        }

        // 🟢 Protected Home Page
        [Route("/Home")]
        public IActionResult Home()
        {
            var userName = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(userName))
            {
                // ❌ Not logged in → redirect to login
                return RedirectToAction("Login_Page");
            }

            ViewBag.Username = userName;
            ViewBag.Email = HttpContext.Session.GetString("UserEmail");
            ViewBag.Role = HttpContext.Session.GetString("UserRole");

            return View();
        }

        // 🟢 Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login_Page");
        }

        // 🔒 Hash Password Method (same as RegisterController)
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}