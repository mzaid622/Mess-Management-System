//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Http;
//using System.Security.Cryptography;
//using System.Text;
//using System.Linq;
//using Mess_Management_System.Models;

//namespace Mess_Management_System.Controllers
//{
//    public class LoginController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public LoginController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public IActionResult Login_Page()
//        {
//            // ? Display success message from registration
//            if (TempData["SuccessMessage"] != null)
//            {
//                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
//            }

//            // ? If user already logged in, redirect to appropriate dashboard
//            var userId = HttpContext.Session.GetString("UserId");
//            if (!string.IsNullOrEmpty(userId))
//            {
//                var role = HttpContext.Session.GetString("UserRole");
//                if (role?.ToLower() == "admin")
//                    return RedirectToAction("Dashboard", "Admin");
//                else
//                    return RedirectToAction("Home", "UserMenu");
//            }

//            return View();
//        }

//        [HttpPost]
//        public IActionResult Login_Page(string email, string password)
//        {
//            // ? Server-side validation - Check if fields are empty
//            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
//            {
//                ViewBag.Error = "Email and Password are required!";
//                return View();
//            }

//            // ? Email format validation
//            if (!IsValidEmail(email))
//            {
//                ViewBag.Error = "Please enter a valid email address!";
//                return View();
//            }

//            // ? Hash password for comparison
//            string hashedPassword = HashPassword(password);

//            // ? Find user by email and password
//            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hashedPassword);

//            if (user != null)
//            {
//                // ? Store user information in session
//                HttpContext.Session.SetString("UserId", user.Id.ToString());
//                HttpContext.Session.SetString("UserName", user.FullName);
//                HttpContext.Session.SetString("UserEmail", user.Email);
//                HttpContext.Session.SetString("UserRole", user.Role);

//                // ? Log successful login (optional - for debugging)
//                Console.WriteLine($"User logged in: {user.Email} - Role: {user.Role}");

//                // ? Redirect based on role
//                if (user.Role.ToLower() == "admin")
//                {
//                    TempData["WelcomeMessage"] = $"Welcome back, {user.FullName}!";
//                    return RedirectToAction("Dashboard", "Admin");
//                }
//                else
//                {
//                    TempData["WelcomeMessage"] = $"Welcome back, {user.FullName}!";
//                    return RedirectToAction("Home", "UserMenu");
//                }
//            }

//            // ? Invalid credentials
//            ViewBag.Error = "Invalid Email or Password!";
//            return View();
//        }

//        // ? Logout with session clear and redirect
//        public IActionResult Logout()
//        {
//            // Clear all session data
//            HttpContext.Session.Clear();

//            // ? Set logout success message
//            TempData["LogoutMessage"] = "You have been logged out successfully!";

//            return RedirectToAction("Login_Page");
//        }

//        // ? Email validation helper method
//        private bool IsValidEmail(string email)
//        {
//            try
//            {
//                var addr = new System.Net.Mail.MailAddress(email);
//                return addr.Address == email;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        // ? Password hashing method
//        private string HashPassword(string password)
//        {
//            using var sha256 = SHA256.Create();
//            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
//            return string.Concat(bytes.Select(b => b.ToString("x2")));
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Mess_Management_System.Models;
using Mess_Management_System.Services;

namespace Mess_Management_System.Controllers
{
    [AllowAnonymous] // ? Allow anonymous access to login/register pages
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public LoginController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpGet]
        public IActionResult Login_Page()
        {
            // ? Display success message from registration
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            // ? Check if user already has valid JWT token
            if (Request.Cookies.ContainsKey("AuthToken"))
            {
                var token = Request.Cookies["AuthToken"];
                var role = _jwtService.GetRoleFromToken(token);

                if (role != null && !_jwtService.IsTokenExpired(token))
                {
                    if (role.ToLower() == "admin")
                        return RedirectToAction("Dashboard", "Admin");
                    else
                        return RedirectToAction("Home", "UserMenu");
                }
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login_Page(string email, string password)
        {
            // ? Server-side validation
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email and Password are required!";
                return View();
            }

            // ? Email format validation
            if (!IsValidEmail(email))
            {
                ViewBag.Error = "Please enter a valid email address!";
                return View();
            }

            // ? Hash password for comparison
            string hashedPassword = HashPassword(password);

            // ? Find user by email and password
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hashedPassword);

            if (user != null)
            {
                // ? Generate JWT token
                var token = _jwtService.GenerateToken(user.Id, user.FullName, user.Email, user.Role);

                // ? Store token in HTTP-only cookie (secure)
                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true, // Prevents JavaScript access (XSS protection)
                    Secure = true, // Only sent over HTTPS
                    SameSite = SameSiteMode.Strict, // CSRF protection
                    Expires = DateTimeOffset.UtcNow.AddHours(24) // Match JWT expiry
                });

                // ? Log successful login
                Console.WriteLine($"User logged in: {user.Email} - Role: {user.Role}");

                // ? Redirect based on role
                if (user.Role.ToLower() == "admin")
                {
                    TempData["WelcomeMessage"] = $"Welcome back, {user.FullName}!";
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    TempData["WelcomeMessage"] = $"Welcome back, {user.FullName}!";
                    return RedirectToAction("Home", "UserMenu");
                }
            }

            // ? Invalid credentials
            ViewBag.Error = "Invalid Email or Password!";
            return View();
        }

        // ? Logout - Clear JWT token
        public IActionResult Logout()
        {
            // Clear JWT token cookie
            Response.Cookies.Delete("AuthToken");

            // ? Set logout success message
            TempData["LogoutMessage"] = "You have been logged out successfully!";

            return RedirectToAction("Login_Page");
        }

        // ? Email validation helper method
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // ? Password hashing method
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return string.Concat(bytes.Select(b => b.ToString("x2")));
        }
    }
}

