using Mess_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Mess_Management_System.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegisterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register_Page()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register_Page(string FullName, string Email, string Password, string ConfirmPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(Email) ||
                    string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
                {
                    ViewBag.Error = "All fields are required.";
                    return View();
                }

                if (Password != ConfirmPassword)
                {
                    ViewBag.Error = "Passwords do not match.";
                    return View();
                }

                string hashedPassword = HashPassword(Password);

                var newUser = new User
                {
                    FullName = FullName,
                    Email = Email,
                    PasswordHash = hashedPassword,
                    Drink = true,
                    Lunch = false,
                    Role = "User"
                };

                _context.Users.Add(newUser);
                int result = _context.SaveChanges();

                if (result == 0)
                {
                    ViewBag.Error = "Failed to save user. No changes detected.";
                    return View();
                }

                return RedirectToAction("Login_Page", "Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error: " + ex.Message;
                return View();
            }
        }  

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
