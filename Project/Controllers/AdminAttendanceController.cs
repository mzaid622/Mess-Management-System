//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Mess_Management_System.Models;
//using System.Linq;

//namespace Mess_Management_System.Controllers
//{
//    public class AdminAttendanceController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public AdminAttendanceController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // ✅ GET: Display Attendance Page
//        public IActionResult Index()
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return RedirectToAction("Login_Page", "Login");
//            }

//            // Get today's menu items
//            var todayMenu = _context.Menus
//                .Where(m => m.Date.Date == DateTime.Now.Date)
//                .OrderBy(m => m.IsFood) // Drinks first, then food
//                .ToList();

//            // Get all users
//            var users = _context.Users
//                .OrderBy(u => u.FullName)
//                .ToList();

//            // ✅ Get existing attendance records for today
//            var todayAttendances = _context.Attendances
//                .Include(a => a.Menu)
//                .Where(a => a.Menu.Date.Date == DateTime.Now.Date)
//                .ToList();

//            // Create view model
//            var model = new AdminAttendanceViewModel
//            {
//                Users = users,
//                TodayMenu = todayMenu,
//                Attendances = todayAttendances // ✅ Pass existing attendance data
//            };

//            return View(model);
//        }

//        // ✅ POST: Mark/Unmark Attendance
//        [HttpPost]
//        public IActionResult MarkAttendance(int userId, int menuId, bool attended)
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return Json(new { success = false, message = "Unauthorized access!" });
//            }

//            try
//            {
//                // Check if attendance record exists
//                var attendance = _context.Attendances
//                    .FirstOrDefault(a => a.UserId == userId && a.MenuId == menuId);

//                if (attendance == null)
//                {
//                    // Create new attendance record
//                    attendance = new Attendance
//                    {
//                        UserId = userId,
//                        MenuId = menuId,
//                        Attended = attended
//                    };
//                    _context.Attendances.Add(attendance);
//                }
//                else
//                {
//                    // Update existing attendance
//                    attendance.Attended = attended;
//                    _context.Attendances.Update(attendance);
//                }

//                _context.SaveChanges();

//                return Json(new { success = true, message = "Attendance updated successfully!" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = "Error updating attendance: " + ex.Message });
//            }
//        }

//        // ✅ POST: Mark All Drinks Automatically (called daily/manually)
//        [HttpPost]
//        public IActionResult AutoMarkDrinks()
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return Json(new { success = false, message = "Unauthorized access!" });
//            }

//            try
//            {
//                // Get today's drink items (IsFood = false)
//                var todayDrinks = _context.Menus
//                    .Where(m => m.Date.Date == DateTime.Now.Date && !m.IsFood)
//                    .ToList();

//                // Get all users
//                var users = _context.Users.ToList();

//                int markedCount = 0;

//                foreach (var user in users)
//                {
//                    foreach (var drink in todayDrinks)
//                    {
//                        // Check if attendance already exists
//                        var existingAttendance = _context.Attendances
//                            .FirstOrDefault(a => a.UserId == user.Id && a.MenuId == drink.Id);

//                        if (existingAttendance == null)
//                        {
//                            // Create new attendance for drink (mandatory)
//                            var attendance = new Attendance
//                            {
//                                UserId = user.Id,
//                                MenuId = drink.Id,
//                                Attended = true // ✅ Drinks are always marked as attended
//                            };
//                            _context.Attendances.Add(attendance);
//                            markedCount++;
//                        }
//                        else if (!existingAttendance.Attended)
//                        {
//                            // Update if it was unmarked
//                            existingAttendance.Attended = true;
//                            _context.Attendances.Update(existingAttendance);
//                            markedCount++;
//                        }
//                    }
//                }

//                _context.SaveChanges();

//                TempData["SuccessMessage"] = $"Successfully auto-marked {markedCount} drink attendances!";
//                return RedirectToAction("Index");
//            }
//            catch (Exception ex)
//            {
//                TempData["ErrorMessage"] = "Error auto-marking drinks: " + ex.Message;
//                return RedirectToAction("Index");
//            }
//        }

//        // ✅ GET: Export Attendance Report (Optional)
//        public IActionResult ExportAttendance()
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return RedirectToAction("Login_Page", "Login");
//            }

//            var today = DateTime.Now.Date;

//            var attendanceData = _context.Attendances
//                .Include(a => a.User)
//                .Include(a => a.Menu)
//                .Where(a => a.Menu.Date.Date == today && a.Attended)
//                .Select(a => new
//                {
//                    UserName = a.User.FullName,
//                    MenuItem = a.Menu.Name,
//                    Price = a.Menu.Price,
//                    Type = a.Menu.IsFood ? "Food" : "Drink"
//                })
//                .ToList();

//            ViewBag.AttendanceData = attendanceData;
//            ViewBag.Date = today.ToString("yyyy-MM-dd");

//            return View();
//        }
//    }
//}



using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Mess_Management_System.Models;
using System.Linq;

namespace Mess_Management_System.Controllers
{
    [Authorize(Roles = "Admin")] // ✅ Only Admin can access
    public class AdminAttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminAttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ GET: Display Attendance Page
        public IActionResult Index()
        {
            // Get today's menu items
            var todayMenu = _context.Menus
                .Where(m => m.Date.Date == DateTime.Now.Date)
                .OrderBy(m => m.IsFood) // Drinks first, then food
                .ToList();

            // Get all users
            var users = _context.Users
                .OrderBy(u => u.FullName)
                .ToList();

            // ✅ Get existing attendance records for today
            var todayAttendances = _context.Attendances
                .Include(a => a.Menu)
                .Where(a => a.Menu.Date.Date == DateTime.Now.Date)
                .ToList();

            // Create view model
            var model = new AdminAttendanceViewModel
            {
                Users = users,
                TodayMenu = todayMenu,
                Attendances = todayAttendances
            };

            return View(model);
        }

        // ✅ POST: Mark/Unmark Attendance
        [HttpPost]
        public IActionResult MarkAttendance(int userId, int menuId, bool attended)
        {
            try
            {
                // Check if attendance record exists
                var attendance = _context.Attendances
                    .FirstOrDefault(a => a.UserId == userId && a.MenuId == menuId);

                if (attendance == null)
                {
                    // Create new attendance record
                    attendance = new Attendance
                    {
                        UserId = userId,
                        MenuId = menuId,
                        Attended = attended
                    };
                    _context.Attendances.Add(attendance);
                }
                else
                {
                    // Update existing attendance
                    attendance.Attended = attended;
                    _context.Attendances.Update(attendance);
                }

                _context.SaveChanges();

                return Json(new { success = true, message = "Attendance updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating attendance: " + ex.Message });
            }
        }

        // ✅ POST: Mark All Drinks Automatically
        [HttpPost]
        public IActionResult AutoMarkDrinks()
        {
            try
            {
                // Get today's drink items (IsFood = false)
                var todayDrinks = _context.Menus
                    .Where(m => m.Date.Date == DateTime.Now.Date && !m.IsFood)
                    .ToList();

                // Get all users
                var users = _context.Users.ToList();

                int markedCount = 0;

                foreach (var user in users)
                {
                    foreach (var drink in todayDrinks)
                    {
                        // Check if attendance already exists
                        var existingAttendance = _context.Attendances
                            .FirstOrDefault(a => a.UserId == user.Id && a.MenuId == drink.Id);

                        if (existingAttendance == null)
                        {
                            // Create new attendance for drink (mandatory)
                            var attendance = new Attendance
                            {
                                UserId = user.Id,
                                MenuId = drink.Id,
                                Attended = true
                            };
                            _context.Attendances.Add(attendance);
                            markedCount++;
                        }
                        else if (!existingAttendance.Attended)
                        {
                            // Update if it was unmarked
                            existingAttendance.Attended = true;
                            _context.Attendances.Update(existingAttendance);
                            markedCount++;
                        }
                    }
                }

                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Successfully auto-marked {markedCount} drink attendances!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error auto-marking drinks: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // ✅ GET: Export Attendance Report
        public IActionResult ExportAttendance()
        {
            var today = DateTime.Now.Date;

            var attendanceData = _context.Attendances
                .Include(a => a.User)
                .Include(a => a.Menu)
                .Where(a => a.Menu.Date.Date == today && a.Attended)
                .Select(a => new
                {
                    UserName = a.User.FullName,
                    MenuItem = a.Menu.Name,
                    Price = a.Menu.Price,
                    Type = a.Menu.IsFood ? "Food" : "Drink"
                })
                .ToList();

            ViewBag.AttendanceData = attendanceData;
            ViewBag.Date = today.ToString("yyyy-MM-dd");

            return View();
        }
    }
}