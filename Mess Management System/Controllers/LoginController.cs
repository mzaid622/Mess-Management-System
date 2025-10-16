using Microsoft.AspNetCore.Mvc;

namespace Mess_Management_System.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login_page()
        {
            return View();
        }
    }
}
