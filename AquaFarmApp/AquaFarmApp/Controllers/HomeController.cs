using AquaFarmApp.Data;
using AquaFarmApp.Models;
using AquaFarmApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AquaFarmApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AquaFarmContext _context;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, AquaFarmContext context, UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Chuyển hướng nếu chưa login
            }

            var isOwner = await _userManager.IsInRoleAsync(user, "Owner");

            // Lấy Farms liên kết với user (n-n)
            var farms = await _context.Farms
                .Include(f => f.Areas)
                .Include(f => f.Users)
                .Where(f => f.Users.Any(u => u.Id == user.Id))
                .ToListAsync();

            int totalFarms = farms.Count;
            int totalAreas = farms.Sum(f => f.Areas.Count);

            int totalStaff = 0;
            if (isOwner)
            {
                // Tổng số nhân viên: User role "Staff" liên kết với Farms của Owner
                totalStaff = await _context.Users
                    .Where(u => u.Role == "Staff" && u.Farms.Any(f => farms.Select(ff => ff.FarmId).Contains(f.FarmId)))
                    .CountAsync();
            }

            var model = new HomeViewModel
            {
                TotalFarms = totalFarms,
                TotalAreas = totalAreas,
                TotalStaff = totalStaff,
                Farms = farms,
                IsOwner = isOwner
            };

            return View(model);
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Admin()
        {
            return View();
        }

        [Authorize(Roles = "Owner")]
        public IActionResult Owner()
        {
            return View();
        }

        [Authorize(Roles = "Staff")]
        public IActionResult Staff()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
