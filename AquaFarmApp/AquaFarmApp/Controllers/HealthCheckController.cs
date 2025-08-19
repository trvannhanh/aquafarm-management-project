using AquaFarmApp.Data;
using AquaFarmApp.Models;
using AquaFarmApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Controllers
{
    public class HealthCheckController : Controller
    {
        private readonly AquaFarmContext _context;
        private readonly UserManager<User> _userManager;

        public HealthCheckController(AquaFarmContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private List<string> GetStatus() => new() { "Normal", "Sick" };
        [HttpGet]
        public IActionResult Create(int id)
        {
            var model = new CreateHealthCheckViewModel
            {
                AreaBatchId = id
            };
            ViewBag.StatusList = GetStatus();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateHealthCheckViewModel model)
        {
            var area = await _context.Areas.FindAsync(model.AreaBatchId);
            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = GetStatus();
                
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Không thể xác định người dùng hiện tại.";
                var areaBatchForView = await _context.AreaBatches
                    .Include(ab => ab.Area)
                    .Include(ab => ab.Batch)
                    .FirstOrDefaultAsync(ab => ab.Id == model.AreaBatchId);
                if (areaBatchForView != null)
                {
                    ViewBag.StatusList = GetStatus();
                }
                return View(model);
            }
            var health = new HealthCheck
            {
                DiseaseSigns = model.DiseaseSigns,
                HealthStatus = model.HealthStatus,
                CheckDate = DateTime.Now,
                Notes = model.Notes,
                UserId = user.Id,
                AreaBatchId = model.AreaBatchId,
                
            };
            _context.HealthChecks.Add(health);
            await _context.SaveChangesAsync();
            return RedirectToAction("AreaBatchIndex", "Batch");
        }
    }
}
