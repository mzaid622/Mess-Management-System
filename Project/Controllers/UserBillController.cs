//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using Mess_Management_System.Models;
//using Mess_Management_System.Services;
//using System.Linq;

//namespace Mess_Management_System.Controllers
//{
//    public class UserBillController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly PdfService _pdfService;

//        public UserBillController(ApplicationDbContext context, PdfService pdfService)
//        {
//            _context = context;
//            _pdfService = pdfService;
//        }

//        // ---------------- User Bill History ----------------
//        public IActionResult Index()
//        {
//            // ✅ Check if user is logged in
//            var userIdString = HttpContext.Session.GetString("UserId");
//            var role = HttpContext.Session.GetString("UserRole");

//            if (string.IsNullOrEmpty(userIdString))
//                return RedirectToAction("Login_Page", "Login");

//            // ✅ Redirect admin to admin dashboard
//            if (role?.ToLower() == "admin")
//                return RedirectToAction("AllBills", "Admin");

//            int userId = int.Parse(userIdString);

//            // ✅ Get all bills for user
//            var bills = _context.Bills
//                .Include(b => b.User)
//                .Where(b => b.UserId == userId)
//                .OrderByDescending(b => b.Date)
//                .ToList();

//            // ✅ Calculate statistics
//            ViewBag.TotalBills = bills.Count;
//            ViewBag.PaidBills = bills.Count(b => b.Paid);
//            ViewBag.UnpaidBills = bills.Count(b => !b.Paid);

//            ViewBag.TotalAmount = bills.Sum(b => b.Amount);
//            ViewBag.PaidAmount = bills.Where(b => b.Paid).Sum(b => b.Amount);
//            ViewBag.UnpaidAmount = bills.Where(b => !b.Paid).Sum(b => b.Amount);

//            // ✅ Group by year for better organization
//            var billsByYear = bills.GroupBy(b => b.Date.Year)
//                                   .OrderByDescending(g => g.Key)
//                                   .ToDictionary(g => g.Key, g => g.ToList());

//            ViewBag.BillsByYear = billsByYear;

//            ViewBag.Username = HttpContext.Session.GetString("UserName");

//            return View(bills);
//        }

//        // ✅ NEW: View Bill Details (breakdown)
//        public IActionResult Details(int id)
//        {
//            // ✅ Check if user is logged in
//            var userIdString = HttpContext.Session.GetString("UserId");
//            var role = HttpContext.Session.GetString("UserRole");

//            if (string.IsNullOrEmpty(userIdString))
//                return RedirectToAction("Login_Page", "Login");

//            // ✅ Redirect admin
//            if (role?.ToLower() == "admin")
//                return RedirectToAction("AllBills", "Admin");

//            int userId = int.Parse(userIdString);

//            // ✅ Get bill
//            var bill = _context.Bills
//                .Include(b => b.User)
//                .FirstOrDefault(b => b.Id == id && b.UserId == userId);

//            if (bill == null)
//            {
//                TempData["ErrorMessage"] = "Bill not found or you don't have permission to view it.";
//                return RedirectToAction("Index");
//            }

//            // ✅ Get attendance records for this bill's month
//            var billMonth = bill.Date.Month;
//            var billYear = bill.Date.Year;

//            var attendances = _context.Attendances
//                .Include(a => a.Menu)
//                .Where(a => a.UserId == userId &&
//                           a.Attended &&
//                           a.Menu.Date.Month == billMonth &&
//                           a.Menu.Date.Year == billYear)
//                .OrderBy(a => a.Menu.Date)
//                .ToList();

//            ViewBag.Attendances = attendances;
//            ViewBag.Username = HttpContext.Session.GetString("UserName");

//            // ✅ Calculate breakdown
//            ViewBag.FoodTotal = attendances.Where(a => a.Menu.IsFood).Sum(a => a.Menu.Price);
//            ViewBag.DrinkTotal = attendances.Where(a => !a.Menu.IsFood).Sum(a => a.Menu.Price);
//            ViewBag.FoodCount = attendances.Count(a => a.Menu.IsFood);
//            ViewBag.DrinkCount = attendances.Count(a => !a.Menu.IsFood);

//            return View(bill);
//        }

//        // ✅ UPDATED: Download Invoice (Returns HTML that can be printed as PDF)
//        public IActionResult DownloadInvoice(int id)
//        {
//            // ✅ Check if user is logged in
//            var userIdString = HttpContext.Session.GetString("UserId");
//            if (string.IsNullOrEmpty(userIdString))
//                return RedirectToAction("Login_Page", "Login");

//            int userId = int.Parse(userIdString);

//            // ✅ Get bill
//            var bill = _context.Bills
//                .Include(b => b.User)
//                .FirstOrDefault(b => b.Id == id && b.UserId == userId);

//            if (bill == null)
//            {
//                TempData["ErrorMessage"] = "Bill not found.";
//                return RedirectToAction("Index");
//            }

//            try
//            {
//                // ✅ Generate HTML Invoice
//                var htmlContent = _pdfService.GenerateBillInvoiceHtml(id, userId);

//                // ✅ Return HTML that user can print as PDF (Ctrl+P -> Save as PDF)
//                return Content(htmlContent, "text/html");
//            }
//            catch (Exception ex)
//            {
//                TempData["ErrorMessage"] = "Error generating PDF: " + ex.Message;
//                return RedirectToAction("Details", new { id = id });
//            }
//        }

//        // ✅ NEW: Filter bills by status
//        public IActionResult FilterByStatus(string status)
//        {
//            // ✅ Check if user is logged in
//            var userIdString = HttpContext.Session.GetString("UserId");
//            var role = HttpContext.Session.GetString("UserRole");

//            if (string.IsNullOrEmpty(userIdString))
//                return RedirectToAction("Login_Page", "Login");

//            if (role?.ToLower() == "admin")
//                return RedirectToAction("AllBills", "Admin");

//            int userId = int.Parse(userIdString);

//            // ✅ Get bills based on filter
//            var bills = _context.Bills
//                .Include(b => b.User)
//                .Where(b => b.UserId == userId)
//                .OrderByDescending(b => b.Date)
//                .ToList();

//            if (status == "paid")
//                bills = bills.Where(b => b.Paid).ToList();
//            else if (status == "unpaid")
//                bills = bills.Where(b => !b.Paid).ToList();

//            // ✅ Calculate statistics
//            ViewBag.TotalBills = bills.Count;
//            ViewBag.PaidBills = bills.Count(b => b.Paid);
//            ViewBag.UnpaidBills = bills.Count(b => !b.Paid);

//            ViewBag.TotalAmount = bills.Sum(b => b.Amount);
//            ViewBag.PaidAmount = bills.Where(b => b.Paid).Sum(b => b.Amount);
//            ViewBag.UnpaidAmount = bills.Where(b => !b.Paid).Sum(b => b.Amount);

//            ViewBag.Username = HttpContext.Session.GetString("UserName");
//            ViewBag.CurrentFilter = status;

//            return View("Index", bills);
//        }

//        // ✅ NEW: Monthly View
//        public IActionResult Monthly(int? month, int? year)
//        {
//            // ✅ Check if user is logged in
//            var userIdString = HttpContext.Session.GetString("UserId");
//            var role = HttpContext.Session.GetString("UserRole");

//            if (string.IsNullOrEmpty(userIdString))
//                return RedirectToAction("Login_Page", "Login");

//            if (role?.ToLower() == "admin")
//                return RedirectToAction("AllBills", "Admin");

//            int userId = int.Parse(userIdString);

//            // Default to current month if not specified
//            var selectedMonth = month ?? DateTime.Now.Month;
//            var selectedYear = year ?? DateTime.Now.Year;

//            // ✅ Get bills for selected month
//            var bills = _context.Bills
//                .Include(b => b.User)
//                .Where(b => b.UserId == userId &&
//                           b.Date.Month == selectedMonth &&
//                           b.Date.Year == selectedYear)
//                .OrderByDescending(b => b.Date)
//                .ToList();

//            ViewBag.SelectedMonth = selectedMonth;
//            ViewBag.SelectedYear = selectedYear;
//            ViewBag.MonthName = new DateTime(selectedYear, selectedMonth, 1).ToString("MMMM yyyy");
//            ViewBag.Username = HttpContext.Session.GetString("UserName");

//            // ✅ Calculate monthly stats
//            ViewBag.MonthlyTotal = bills.Sum(b => b.Amount);
//            ViewBag.MonthlyPaid = bills.Where(b => b.Paid).Sum(b => b.Amount);
//            ViewBag.MonthlyUnpaid = bills.Where(b => !b.Paid).Sum(b => b.Amount);

//            return View(bills);
//        }
//    }
//}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Mess_Management_System.Models;
using Mess_Management_System.Services;
using System.Linq;
using System.Security.Claims;

namespace Mess_Management_System.Controllers
{
    [Authorize(Roles = "User")] // ✅ Only regular users can access
    public class UserBillController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PdfService _pdfService;

        public UserBillController(ApplicationDbContext context, PdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // ✅ Helper methods to get current user info from JWT claims
        private int GetCurrentUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        private string GetCurrentUserName() => User.FindFirst(ClaimTypes.Name)?.Value ?? "User";

        // ---------------- User Bill History ----------------
        public IActionResult Index()
        {
            int userId = GetCurrentUserId();

            // ✅ Get all bills for user
            var bills = _context.Bills
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Date)
                .ToList();

            // ✅ Calculate statistics
            ViewBag.TotalBills = bills.Count;
            ViewBag.PaidBills = bills.Count(b => b.Paid);
            ViewBag.UnpaidBills = bills.Count(b => !b.Paid);

            ViewBag.TotalAmount = bills.Sum(b => b.Amount);
            ViewBag.PaidAmount = bills.Where(b => b.Paid).Sum(b => b.Amount);
            ViewBag.UnpaidAmount = bills.Where(b => !b.Paid).Sum(b => b.Amount);

            // ✅ Group by year for better organization
            var billsByYear = bills.GroupBy(b => b.Date.Year)
                                   .OrderByDescending(g => g.Key)
                                   .ToDictionary(g => g.Key, g => g.ToList());

            ViewBag.BillsByYear = billsByYear;
            ViewBag.Username = GetCurrentUserName();

            return View(bills);
        }

        // ✅ View Bill Details (breakdown)
        public IActionResult Details(int id)
        {
            int userId = GetCurrentUserId();

            // ✅ Get bill
            var bill = _context.Bills
                .Include(b => b.User)
                .FirstOrDefault(b => b.Id == id && b.UserId == userId);

            if (bill == null)
            {
                TempData["ErrorMessage"] = "Bill not found or you don't have permission to view it.";
                return RedirectToAction("Index");
            }

            // ✅ Get attendance records for this bill's month
            var billMonth = bill.Date.Month;
            var billYear = bill.Date.Year;

            var attendances = _context.Attendances
                .Include(a => a.Menu)
                .Where(a => a.UserId == userId &&
                           a.Attended &&
                           a.Menu.Date.Month == billMonth &&
                           a.Menu.Date.Year == billYear)
                .OrderBy(a => a.Menu.Date)
                .ToList();

            ViewBag.Attendances = attendances;
            ViewBag.Username = GetCurrentUserName();

            // ✅ Calculate breakdown
            ViewBag.FoodTotal = attendances.Where(a => a.Menu.IsFood).Sum(a => a.Menu.Price);
            ViewBag.DrinkTotal = attendances.Where(a => !a.Menu.IsFood).Sum(a => a.Menu.Price);
            ViewBag.FoodCount = attendances.Count(a => a.Menu.IsFood);
            ViewBag.DrinkCount = attendances.Count(a => !a.Menu.IsFood);

            return View(bill);
        }

        // ✅ Download Invoice
        public IActionResult DownloadInvoice(int id)
        {
            int userId = GetCurrentUserId();

            // ✅ Get bill
            var bill = _context.Bills
                .Include(b => b.User)
                .FirstOrDefault(b => b.Id == id && b.UserId == userId);

            if (bill == null)
            {
                TempData["ErrorMessage"] = "Bill not found.";
                return RedirectToAction("Index");
            }

            try
            {
                // ✅ Generate HTML Invoice
                var htmlContent = _pdfService.GenerateBillInvoiceHtml(id, userId);
                return Content(htmlContent, "text/html");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error generating PDF: " + ex.Message;
                return RedirectToAction("Details", new { id = id });
            }
        }

        // ✅ Filter bills by status
        public IActionResult FilterByStatus(string status)
        {
            int userId = GetCurrentUserId();

            // ✅ Get bills based on filter
            var bills = _context.Bills
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Date)
                .ToList();

            if (status == "paid")
                bills = bills.Where(b => b.Paid).ToList();
            else if (status == "unpaid")
                bills = bills.Where(b => !b.Paid).ToList();

            // ✅ Calculate statistics
            ViewBag.TotalBills = bills.Count;
            ViewBag.PaidBills = bills.Count(b => b.Paid);
            ViewBag.UnpaidBills = bills.Count(b => !b.Paid);

            ViewBag.TotalAmount = bills.Sum(b => b.Amount);
            ViewBag.PaidAmount = bills.Where(b => b.Paid).Sum(b => b.Amount);
            ViewBag.UnpaidAmount = bills.Where(b => !b.Paid).Sum(b => b.Amount);

            ViewBag.Username = GetCurrentUserName();
            ViewBag.CurrentFilter = status;

            return View("Index", bills);
        }

        // ✅ Monthly View
        public IActionResult Monthly(int? month, int? year)
        {
            int userId = GetCurrentUserId();

            // Default to current month if not specified
            var selectedMonth = month ?? DateTime.Now.Month;
            var selectedYear = year ?? DateTime.Now.Year;

            // ✅ Get bills for selected month
            var bills = _context.Bills
                .Include(b => b.User)
                .Where(b => b.UserId == userId &&
                           b.Date.Month == selectedMonth &&
                           b.Date.Year == selectedYear)
                .OrderByDescending(b => b.Date)
                .ToList();

            ViewBag.SelectedMonth = selectedMonth;
            ViewBag.SelectedYear = selectedYear;
            ViewBag.MonthName = new DateTime(selectedYear, selectedMonth, 1).ToString("MMMM yyyy");
            ViewBag.Username = GetCurrentUserName();

            // ✅ Calculate monthly stats
            ViewBag.MonthlyTotal = bills.Sum(b => b.Amount);
            ViewBag.MonthlyPaid = bills.Where(b => b.Paid).Sum(b => b.Amount);
            ViewBag.MonthlyUnpaid = bills.Where(b => !b.Paid).Sum(b => b.Amount);

            return View(bills);
        }
    }
}