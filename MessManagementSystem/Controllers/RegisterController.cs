using Microsoft.AspNetCore.Mvc;

namespace Mess_Management_System.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Register_Page()
        {
            return View();
        }
    }
}
