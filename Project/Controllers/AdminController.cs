//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using Mess_Management_System.Models;
//using Mess_Management_System.Services;
//using System;
//using System.Linq;

//namespace Mess_Management_System.Controllers
//{
//    public class AdminController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly PdfService _pdfService;

//        public AdminController(ApplicationDbContext context, PdfService pdfService)
//        {
//            _context = context;
//            _pdfService = pdfService;
//        }

//        // ---------------- Dashboard ----------------
//        public IActionResult Dashboard()
//        {
//            // ✅ Check if admin is logged in
//            var userName = HttpContext.Session.GetString("UserName");
//            var role = HttpContext.Session.GetString("UserRole");

//            if (string.IsNullOrEmpty(userName))
//                return RedirectToAction("Login_Page", "Login");

//            if (role?.ToLower() != "admin")
//                return RedirectToAction("Home", "UserMenu");

//            // ✅ Display welcome message if exists
//            if (TempData["WelcomeMessage"] != null)
//            {
//                ViewBag.WelcomeMessage = TempData["WelcomeMessage"].ToString();
//            }

//            ViewBag.Username = userName;
//            ViewBag.Email = HttpContext.Session.GetString("UserEmail");
//            ViewBag.Role = role;

//            // ✅ Quick Stats
//            ViewBag.TotalUsers = _context.Users.Count();
//            ViewBag.TodayFood = _context.Menus.Count(m => m.Date.Date == DateTime.Now.Date && m.IsFood);
//            ViewBag.TodayDrink = _context.Menus.Count(m => m.Date.Date == DateTime.Now.Date && !m.IsFood);

//            // ✅ Additional Stats
//            ViewBag.TotalMenuItems = _context.Menus.Count();
//            ViewBag.TodayAttendance = _context.Attendances
//                .Include(a => a.Menu)
//                .Count(a => a.Menu.Date.Date == DateTime.Now.Date && a.Attended);
//            ViewBag.UnpaidBills = _context.Bills.Count(b => !b.Paid);
//            ViewBag.TotalRevenue = _context.Bills.Where(b => b.Paid).Sum(b => b.Amount);
//            ViewBag.PendingRevenue = _context.Bills.Where(b => !b.Paid).Sum(b => b.Amount);

//            return View();
//        }

//        // ---------------- Generate Monthly Bills ----------------
//        [HttpPost]
//        public IActionResult GenerateMonthlyBills(string period = "current")
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return RedirectToAction("Login_Page", "Login");
//            }

//            try
//            {
//                var users = _context.Users.ToList();
//                var today = DateTime.Now;

//                // ✅ Calculate for CURRENT month or PREVIOUS month based on parameter
//                DateTime firstDay, lastDay;
//                if (period == "previous")
//                {
//                    firstDay = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
//                    lastDay = firstDay.AddMonths(1).AddDays(-1);
//                }
//                else // current month
//                {
//                    firstDay = new DateTime(today.Year, today.Month, 1);
//                    lastDay = today; // Up to today
//                }

//                int billsGenerated = 0;
//                int billsUpdated = 0;

//                foreach (var user in users)
//                {
//                    // ✅ Get all attendance records for the period
//                    var attendances = _context.Attendances
//                        .Include(a => a.Menu)
//                        .Where(a => a.UserId == user.Id &&
//                                   a.Attended &&
//                                   a.Menu.Date >= firstDay &&
//                                   a.Menu.Date <= lastDay)
//                        .ToList();

//                    // ✅ Calculate total amount based on attendance
//                    decimal totalAmount = attendances.Sum(a => a.Menu.Price);

//                    // ✅ Also add mandatory drinks that weren't marked
//                    var drinksInPeriod = _context.Menus
//                        .Where(m => !m.IsFood && m.Date >= firstDay && m.Date <= lastDay)
//                        .ToList();

//                    foreach (var drink in drinksInPeriod)
//                    {
//                        var drinkAttended = attendances.Any(a => a.MenuId == drink.Id);
//                        if (!drinkAttended)
//                        {
//                            // ✅ Drink not in attendance but should be billed (mandatory)
//                            totalAmount += drink.Price;
//                        }
//                    }

//                    // ✅ Check if bill already exists for this month
//                    var existingBill = _context.Bills
//                        .FirstOrDefault(b => b.UserId == user.Id &&
//                                           b.Date.Month == firstDay.Month &&
//                                           b.Date.Year == firstDay.Year);

//                    if (existingBill == null && totalAmount > 0)
//                    {
//                        // ✅ Create new bill only if there's an amount
//                        var bill = new Bill
//                        {
//                            UserId = user.Id,
//                            Amount = totalAmount,
//                            Date = firstDay, // Use first day of billing month
//                            Paid = false
//                        };
//                        _context.Bills.Add(bill);
//                        billsGenerated++;
//                    }
//                    else if (existingBill != null)
//                    {
//                        // ✅ Update existing bill amount
//                        existingBill.Amount = totalAmount;
//                        _context.Bills.Update(existingBill);
//                        billsUpdated++;
//                    }
//                }

//                _context.SaveChanges();

//                var periodName = period == "previous" ? "previous month" : "current month (up to today)";
//                TempData["SuccessMessage"] = $"Bills for {periodName} processed! Generated: {billsGenerated}, Updated: {billsUpdated}";
//            }
//            catch (Exception ex)
//            {
//                TempData["ErrorMessage"] = "Error generating bills: " + ex.Message;
//            }

//            return RedirectToAction("AllBills");
//        }

//        // ---------------- View All Bills ----------------
//        public IActionResult AllBills()
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return RedirectToAction("Login_Page", "Login");
//            }

//            var bills = _context.Bills
//                .Include(b => b.User)
//                .OrderByDescending(b => b.Date)
//                .ThenBy(b => b.User.FullName)
//                .ToList();

//            // ✅ Calculate summary stats
//            ViewBag.TotalBills = bills.Count;
//            ViewBag.PaidBills = bills.Count(b => b.Paid);
//            ViewBag.UnpaidBills = bills.Count(b => !b.Paid);
//            ViewBag.TotalAmount = bills.Sum(b => b.Amount);
//            ViewBag.PaidAmount = bills.Where(b => b.Paid).Sum(b => b.Amount);
//            ViewBag.UnpaidAmount = bills.Where(b => !b.Paid).Sum(b => b.Amount);

//            return View(bills);
//        }

//        // ---------------- Mark Bill Paid ----------------
//        [HttpPost]
//        public IActionResult MarkPaid(int billId)
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return Json(new { success = false, message = "Unauthorized access!" });
//            }

//            try
//            {
//                var bill = _context.Bills
//                    .Include(b => b.User)
//                    .FirstOrDefault(b => b.Id == billId);

//                if (bill != null)
//                {
//                    bill.Paid = true;
//                    _context.Bills.Update(bill);
//                    _context.SaveChanges();

//                    return Json(new
//                    {
//                        success = true,
//                        message = $"Bill for {bill.User.FullName} marked as paid successfully!"
//                    });
//                }

//                return Json(new { success = false, message = "Bill not found!" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = "Error: " + ex.Message });
//            }
//        }

//        // ---------------- Mark Bill Unpaid ----------------
//        [HttpPost]
//        public IActionResult MarkUnpaid(int billId)
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return Json(new { success = false, message = "Unauthorized access!" });
//            }

//            try
//            {
//                var bill = _context.Bills
//                    .Include(b => b.User)
//                    .FirstOrDefault(b => b.Id == billId);

//                if (bill != null)
//                {
//                    bill.Paid = false;
//                    _context.Bills.Update(bill);
//                    _context.SaveChanges();

//                    return Json(new
//                    {
//                        success = true,
//                        message = $"Bill for {bill.User.FullName} marked as unpaid."
//                    });
//                }

//                return Json(new { success = false, message = "Bill not found!" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = "Error: " + ex.Message });
//            }
//        }

//        // ✅ NEW: Delete Bill
//        [HttpPost]
//        public IActionResult DeleteBill(int billId)
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return Json(new { success = false, message = "Unauthorized access!" });
//            }

//            try
//            {
//                var bill = _context.Bills.FirstOrDefault(b => b.Id == billId);

//                if (bill != null)
//                {
//                    _context.Bills.Remove(bill);
//                    _context.SaveChanges();

//                    return Json(new { success = true, message = "Bill deleted successfully!" });
//                }

//                return Json(new { success = false, message = "Bill not found!" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = "Error: " + ex.Message });
//            }
//        }

//        // ✅ NEW: View Monthly Summary Report
//        public IActionResult MonthlyReport(int? month, int? year)
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return RedirectToAction("Login_Page", "Login");
//            }

//            // Default to current month if not specified
//            var selectedMonth = month ?? DateTime.Now.Month;
//            var selectedYear = year ?? DateTime.Now.Year;

//            var bills = _context.Bills
//                .Include(b => b.User)
//                .Where(b => b.Date.Month == selectedMonth && b.Date.Year == selectedYear)
//                .OrderBy(b => b.User.FullName)
//                .ToList();

//            ViewBag.SelectedMonth = selectedMonth;
//            ViewBag.SelectedYear = selectedYear;
//            ViewBag.MonthName = new DateTime(selectedYear, selectedMonth, 1).ToString("MMMM yyyy");

//            return View(bills);
//        }

//        // ✅ NEW: Export Monthly Report as HTML/PDF
//        public IActionResult ExportMonthlyReport(int? month, int? year)
//        {
//            // ✅ Check if admin is logged in
//            var role = HttpContext.Session.GetString("UserRole");
//            if (string.IsNullOrEmpty(role) || role.ToLower() != "admin")
//            {
//                return RedirectToAction("Login_Page", "Login");
//            }

//            var selectedMonth = month ?? DateTime.Now.Month;
//            var selectedYear = year ?? DateTime.Now.Year;

//            try
//            {
//                // ✅ Generate HTML Report
//                var htmlContent = _pdfService.GenerateMonthlyReportHtml(selectedMonth, selectedYear);

//                // ✅ Return HTML that can be printed as PDF (Ctrl+P -> Save as PDF)
//                return Content(htmlContent, "text/html");
//            }
//            catch (Exception ex)
//            {
//                TempData["ErrorMessage"] = "Error generating PDF: " + ex.Message;
//                return RedirectToAction("AllBills");
//            }
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Mess_Management_System.Models;
using Mess_Management_System.Services;
using System;
using System.Linq;
using System.Security.Claims;

namespace Mess_Management_System.Controllers
{
    [Authorize(Roles = "Admin")] // ✅ Only Admin can access
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PdfService _pdfService;

        public AdminController(ApplicationDbContext context, PdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // ✅ Helper method to get current user info from JWT claims
        private string GetCurrentUserName() => User.FindFirst(ClaimTypes.Name)?.Value ?? "Admin";
        private string GetCurrentUserEmail() => User.FindFirst(ClaimTypes.Email)?.Value ?? "";
        private string GetCurrentUserRole() => User.FindFirst(ClaimTypes.Role)?.Value ?? "Admin";

        // ---------------- Dashboard ----------------
        public IActionResult Dashboard()
        {
            // ✅ Get user info from JWT claims
            ViewBag.Username = GetCurrentUserName();
            ViewBag.Email = GetCurrentUserEmail();
            ViewBag.Role = GetCurrentUserRole();

            // ✅ Display welcome message if exists
            if (TempData["WelcomeMessage"] != null)
            {
                ViewBag.WelcomeMessage = TempData["WelcomeMessage"].ToString();
            }

            // ✅ Quick Stats
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.TodayFood = _context.Menus.Count(m => m.Date.Date == DateTime.Now.Date && m.IsFood);
            ViewBag.TodayDrink = _context.Menus.Count(m => m.Date.Date == DateTime.Now.Date && !m.IsFood);

            // ✅ Additional Stats
            ViewBag.TotalMenuItems = _context.Menus.Count();
            ViewBag.TodayAttendance = _context.Attendances
                .Include(a => a.Menu)
                .Count(a => a.Menu.Date.Date == DateTime.Now.Date && a.Attended);
            ViewBag.UnpaidBills = _context.Bills.Count(b => !b.Paid);
            ViewBag.TotalRevenue = _context.Bills.Where(b => b.Paid).Sum(b => b.Amount);
            ViewBag.PendingRevenue = _context.Bills.Where(b => !b.Paid).Sum(b => b.Amount);

            return View();
        }

        // ---------------- Generate Monthly Bills ----------------
        [HttpPost]
        public IActionResult GenerateMonthlyBills(string period = "current")
        {
            try
            {
                var users = _context.Users.ToList();
                var today = DateTime.Now;

                // ✅ Calculate for CURRENT month or PREVIOUS month
                DateTime firstDay, lastDay;
                if (period == "previous")
                {
                    firstDay = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
                    lastDay = firstDay.AddMonths(1).AddDays(-1);
                }
                else // current month
                {
                    firstDay = new DateTime(today.Year, today.Month, 1);
                    lastDay = today;
                }

                int billsGenerated = 0;
                int billsUpdated = 0;

                foreach (var user in users)
                {
                    // ✅ Get all attendance records for the period
                    var attendances = _context.Attendances
                        .Include(a => a.Menu)
                        .Where(a => a.UserId == user.Id &&
                                   a.Attended &&
                                   a.Menu.Date >= firstDay &&
                                   a.Menu.Date <= lastDay)
                        .ToList();

                    // ✅ Calculate total amount
                    decimal totalAmount = attendances.Sum(a => a.Menu.Price);

                    // ✅ Add mandatory drinks
                    var drinksInPeriod = _context.Menus
                        .Where(m => !m.IsFood && m.Date >= firstDay && m.Date <= lastDay)
                        .ToList();

                    foreach (var drink in drinksInPeriod)
                    {
                        var drinkAttended = attendances.Any(a => a.MenuId == drink.Id);
                        if (!drinkAttended)
                        {
                            totalAmount += drink.Price;
                        }
                    }

                    // ✅ Check if bill exists
                    var existingBill = _context.Bills
                        .FirstOrDefault(b => b.UserId == user.Id &&
                                           b.Date.Month == firstDay.Month &&
                                           b.Date.Year == firstDay.Year);

                    if (existingBill == null && totalAmount > 0)
                    {
                        var bill = new Bill
                        {
                            UserId = user.Id,
                            Amount = totalAmount,
                            Date = firstDay,
                            Paid = false
                        };
                        _context.Bills.Add(bill);
                        billsGenerated++;
                    }
                    else if (existingBill != null)
                    {
                        existingBill.Amount = totalAmount;
                        _context.Bills.Update(existingBill);
                        billsUpdated++;
                    }
                }

                _context.SaveChanges();

                var periodName = period == "previous" ? "previous month" : "current month (up to today)";
                TempData["SuccessMessage"] = $"Bills for {periodName} processed! Generated: {billsGenerated}, Updated: {billsUpdated}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error generating bills: " + ex.Message;
            }

            return RedirectToAction("AllBills");
        }

        // ---------------- View All Bills ----------------
        public IActionResult AllBills()
        {
            var bills = _context.Bills
                .Include(b => b.User)
                .OrderByDescending(b => b.Date)
                .ThenBy(b => b.User.FullName)
                .ToList();

            // ✅ Calculate summary stats
            ViewBag.TotalBills = bills.Count;
            ViewBag.PaidBills = bills.Count(b => b.Paid);
            ViewBag.UnpaidBills = bills.Count(b => !b.Paid);
            ViewBag.TotalAmount = bills.Sum(b => b.Amount);
            ViewBag.PaidAmount = bills.Where(b => b.Paid).Sum(b => b.Amount);
            ViewBag.UnpaidAmount = bills.Where(b => !b.Paid).Sum(b => b.Amount);

            return View(bills);
        }

        // ---------------- Mark Bill Paid ----------------
        [HttpPost]
        public IActionResult MarkPaid(int billId)
        {
            try
            {
                var bill = _context.Bills
                    .Include(b => b.User)
                    .FirstOrDefault(b => b.Id == billId);

                if (bill != null)
                {
                    bill.Paid = true;
                    _context.Bills.Update(bill);
                    _context.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        message = $"Bill for {bill.User.FullName} marked as paid successfully!"
                    });
                }

                return Json(new { success = false, message = "Bill not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // ---------------- Mark Bill Unpaid ----------------
        [HttpPost]
        public IActionResult MarkUnpaid(int billId)
        {
            try
            {
                var bill = _context.Bills
                    .Include(b => b.User)
                    .FirstOrDefault(b => b.Id == billId);

                if (bill != null)
                {
                    bill.Paid = false;
                    _context.Bills.Update(bill);
                    _context.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        message = $"Bill for {bill.User.FullName} marked as unpaid."
                    });
                }

                return Json(new { success = false, message = "Bill not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // ✅ Delete Bill
        [HttpPost]
        public IActionResult DeleteBill(int billId)
        {
            try
            {
                var bill = _context.Bills.FirstOrDefault(b => b.Id == billId);

                if (bill != null)
                {
                    _context.Bills.Remove(bill);
                    _context.SaveChanges();

                    return Json(new { success = true, message = "Bill deleted successfully!" });
                }

                return Json(new { success = false, message = "Bill not found!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // ✅ View Monthly Summary Report
        public IActionResult MonthlyReport(int? month, int? year)
        {
            var selectedMonth = month ?? DateTime.Now.Month;
            var selectedYear = year ?? DateTime.Now.Year;

            var bills = _context.Bills
                .Include(b => b.User)
                .Where(b => b.Date.Month == selectedMonth && b.Date.Year == selectedYear)
                .OrderBy(b => b.User.FullName)
                .ToList();

            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.SelectedYear = selectedYear;
            ViewBag.MonthName = new DateTime(selectedYear, selectedMonth, 1).ToString("MMMM yyyy");

            return View(bills);
        }

        // ✅ Export Monthly Report
        public IActionResult ExportMonthlyReport(int? month, int? year)
        {
            var selectedMonth = month ?? DateTime.Now.Month;
            var selectedYear = year ?? DateTime.Now.Year;

            try
            {
                var htmlContent = _pdfService.GenerateMonthlyReportHtml(selectedMonth, selectedYear);
                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error generating PDF: " + ex.Message;
                return RedirectToAction("AllBills");
            }
        }
    }
}

