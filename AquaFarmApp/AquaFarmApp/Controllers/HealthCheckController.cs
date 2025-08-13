using Microsoft.AspNetCore.Mvc;
using AquaFarmApp.Data;
using AquaFarmApp.Models;
using Microsoft.EntityFrameworkCore;
using AquaFarmApp.ViewModels;
using Microsoft.VisualBasic;

namespace AquaFarmApp.Controllers
{
    public class HealthCheckController : Controller
    {
        private readonly AquaFarmContext _context;

        public HealthCheckController(AquaFarmContext context)
        {
            _context = context;
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
            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = GetStatus();
                return View(model);
            }
            var health = new HealthCheck
            {
                DiseaseSigns = model.DiseaseSigns,
                HealthStatus = model.HealthStatus,
                CheckDate = DateTime.Now,
                Notes = model.Notes,
                AreaBatchId = model.AreaBatchId,
                
            };
            _context.HealthChecks.Add(health);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Area");
        }
    }
}
