//using Mess_Management_System.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Mess_Management_System.Controllers
//{
//    public class AdminMenuController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public AdminMenuController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // ✅ List All Menu Items
//        public IActionResult Index()
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return RedirectToAction("Login_Page", "Login");
//            }

//            var menu = _context.Menus.OrderByDescending(m => m.Date).ToList();
//            return View(menu);
//        }

//        // ✅ Add Menu GET
//        [HttpGet]
//        public IActionResult Add()
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return RedirectToAction("Login_Page", "Login");
//            }

//            return View();
//        }

//        // ✅ Add Menu POST with Server-Side Validation
//        [HttpPost]
//        public IActionResult Add(Menu menu)
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return RedirectToAction("Login_Page", "Login");
//            }

//            // ✅ Server-side validation
//            if (string.IsNullOrWhiteSpace(menu.Name))
//            {
//                ViewBag.Error = "Menu name is required.";
//                return View(menu);
//            }

//            if (menu.Price <= 0)
//            {
//                ViewBag.Error = "Price must be greater than 0.";
//                return View(menu);
//            }

//            if (menu.Date.Date < DateTime.Now.Date)
//            {
//                ViewBag.Error = "Date cannot be in the past.";
//                return View(menu);
//            }

//            // ✅ Check for duplicate menu item (same name, same date)
//            var existingMenu = _context.Menus
//                .FirstOrDefault(m => m.Name == menu.Name && m.Date.Date == menu.Date.Date);

//            if (existingMenu != null)
//            {
//                ViewBag.Error = $"A menu item '{menu.Name}' already exists for {menu.Date.ToShortDateString()}.";
//                return View(menu);
//            }

//            // ✅ Validate ModelState
//            if (!ModelState.IsValid)
//            {
//                ViewBag.Error = "Please fill all required fields correctly.";
//                return View(menu);
//            }

//            _context.Menus.Add(menu);
//            _context.SaveChanges();

//            TempData["SuccessMessage"] = "Menu item added successfully!";
//            return RedirectToAction("Index");
//        }

//        // ✅ Edit Menu GET
//        public IActionResult Edit(int id)
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return RedirectToAction("Login_Page", "Login");
//            }

//            var menu = _context.Menus.Find(id);
//            if (menu == null)
//            {
//                TempData["ErrorMessage"] = "Menu item not found.";
//                return RedirectToAction("Index");
//            }

//            return View(menu);
//        }

//        // ✅ Edit Menu POST with Server-Side Validation
//        [HttpPost]
//        public IActionResult Edit(Menu menu)
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return RedirectToAction("Login_Page", "Login");
//            }

//            // ✅ Server-side validation
//            if (string.IsNullOrWhiteSpace(menu.Name))
//            {
//                ViewBag.Error = "Menu name is required.";
//                return View(menu);
//            }

//            if (menu.Price <= 0)
//            {
//                ViewBag.Error = "Price must be greater than 0.";
//                return View(menu);
//            }

//            if (menu.Date.Date < DateTime.Now.Date)
//            {
//                ViewBag.Error = "Date cannot be in the past.";
//                return View(menu);
//            }

//            // ✅ Check for duplicate menu item (same name, same date, different ID)
//            var existingMenu = _context.Menus
//                .FirstOrDefault(m => m.Name == menu.Name && m.Date.Date == menu.Date.Date && m.Id != menu.Id);

//            if (existingMenu != null)
//            {
//                ViewBag.Error = $"A menu item '{menu.Name}' already exists for {menu.Date.ToShortDateString()}.";
//                return View(menu);
//            }

//            // ✅ Validate ModelState
//            if (!ModelState.IsValid)
//            {
//                ViewBag.Error = "Please fill all required fields correctly.";
//                return View(menu);
//            }

//            _context.Menus.Update(menu);
//            _context.SaveChanges();

//            TempData["SuccessMessage"] = "Menu item updated successfully!";
//            return RedirectToAction("Index");
//        }

//        // ✅ Delete Menu with Confirmation
//        public IActionResult Delete(int id)
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return RedirectToAction("Login_Page", "Login");
//            }

//            var menu = _context.Menus.Find(id);
//            if (menu == null)
//            {
//                TempData["ErrorMessage"] = "Menu item not found.";
//                return RedirectToAction("Index");
//            }

//            // ✅ Check if menu has attendance records
//            var hasAttendance = _context.Attendances.Any(a => a.MenuId == id);
//            if (hasAttendance)
//            {
//                TempData["ErrorMessage"] = "Cannot delete menu item. Attendance records exist for this item.";
//                return RedirectToAction("Index");
//            }

//            _context.Menus.Remove(menu);
//            _context.SaveChanges();

//            TempData["SuccessMessage"] = "Menu item deleted successfully!";
//            return RedirectToAction("Index");
//        }
//    }
//}



using Mess_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Mess_Management_System.Controllers
{
    [Authorize(Roles = "Admin")] // ✅ Only Admin can access
    public class AdminMenuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminMenuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ List All Menu Items
        public IActionResult Index()
        {
            var menu = _context.Menus.OrderByDescending(m => m.Date).ToList();
            return View(menu);
        }

        // ✅ Add Menu GET
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        // ✅ Add Menu POST with Server-Side Validation
        [HttpPost]
        public IActionResult Add(Menu menu)
        {
            // ✅ Server-side validation
            if (string.IsNullOrWhiteSpace(menu.Name))
            {
                ViewBag.Error = "Menu name is required.";
                return View(menu);
            }

            if (menu.Price <= 0)
            {
                ViewBag.Error = "Price must be greater than 0.";
                return View(menu);
            }

            if (menu.Date.Date < DateTime.Now.Date)
            {
                ViewBag.Error = "Date cannot be in the past.";
                return View(menu);
            }

            // ✅ Check for duplicate menu item (same name, same date)
            var existingMenu = _context.Menus
                .FirstOrDefault(m => m.Name == menu.Name && m.Date.Date == menu.Date.Date);

            if (existingMenu != null)
            {
                ViewBag.Error = $"A menu item '{menu.Name}' already exists for {menu.Date.ToShortDateString()}.";
                return View(menu);
            }

            // ✅ Validate ModelState
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please fill all required fields correctly.";
                return View(menu);
            }

            _context.Menus.Add(menu);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Menu item added successfully!";
            return RedirectToAction("Index");
        }

        // ✅ Edit Menu GET
        public IActionResult Edit(int id)
        {
            var menu = _context.Menus.Find(id);
            if (menu == null)
            {
                TempData["ErrorMessage"] = "Menu item not found.";
                return RedirectToAction("Index");
            }

            return View(menu);
        }

        // ✅ Edit Menu POST with Server-Side Validation
        [HttpPost]
        public IActionResult Edit(Menu menu)
        {
            // ✅ Server-side validation
            if (string.IsNullOrWhiteSpace(menu.Name))
            {
                ViewBag.Error = "Menu name is required.";
                return View(menu);
            }

            if (menu.Price <= 0)
            {
                ViewBag.Error = "Price must be greater than 0.";
                return View(menu);
            }

            if (menu.Date.Date < DateTime.Now.Date)
            {
                ViewBag.Error = "Date cannot be in the past.";
                return View(menu);
            }

            // ✅ Check for duplicate menu item (same name, same date, different ID)
            var existingMenu = _context.Menus
                .FirstOrDefault(m => m.Name == menu.Name && m.Date.Date == menu.Date.Date && m.Id != menu.Id);

            if (existingMenu != null)
            {
                ViewBag.Error = $"A menu item '{menu.Name}' already exists for {menu.Date.ToShortDateString()}.";
                return View(menu);
            }

            // ✅ Validate ModelState
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Please fill all required fields correctly.";
                return View(menu);
            }

            _context.Menus.Update(menu);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Menu item updated successfully!";
            return RedirectToAction("Index");
        }

        // ✅ Delete Menu with Confirmation
        public IActionResult Delete(int id)
        {
            var menu = _context.Menus.Find(id);
            if (menu == null)
            {
                TempData["ErrorMessage"] = "Menu item not found.";
                return RedirectToAction("Index");
            }

            // ✅ Check if menu has attendance records
            var hasAttendance = _context.Attendances.Any(a => a.MenuId == id);
            if (hasAttendance)
            {
                TempData["ErrorMessage"] = "Cannot delete menu item. Attendance records exist for this item.";
                return RedirectToAction("Index");
            }

            _context.Menus.Remove(menu);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Menu item deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
