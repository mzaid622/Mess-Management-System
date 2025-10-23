using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Mess_Management_System.Controllers
{
    public class LoginController : Controller
    {
        // 🟢 Show Login Page
        public IActionResult Login_Page()
        {
            return View();
        }

        // 🟢 Handle Login Form Submission
        [HttpPost]
        public IActionResult Login_Page(string username, string password)
        {
            // ✅ Check credentials
            if (username == "admin" && password == "12345")
            {
                // ✅ Save the username in session
                HttpContext.Session.SetString("User", username);

                // ✅ Redirect to Home
                return RedirectToAction("Home");
            }
            else
            {
                // ❌ Wrong credentials
                ViewBag.Error = "Invalid username or password!";
                return View();  // <— Return same view, not RedirectToAction("Login")
            }
        }

        // 🟢 Protected Home Page
        [Route("/Home")]
        public IActionResult Home()
        {
            var user = HttpContext.Session.GetString("User");

            if (string.IsNullOrEmpty(user))
            {
                // ❌ Not logged in → redirect to login
                return RedirectToAction("Login_Page");
            }

            ViewBag.Username = user;
            return View();
        }

        
    }
}
