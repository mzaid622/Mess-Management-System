using Microsoft.AspNetCore.Mvc;

namespace Mess_Management_System.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login_Page()
        {
            return View();
        }

        [Route("/Home")]
        public IActionResult Home()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login_Page(string username, string password)
        {
            // Check if credentials are correct
            if (username == "admin" && password == "12345")
            {
                // Redirect to Home if login successful
                return Redirect("/Home");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}
