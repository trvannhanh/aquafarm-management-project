using Microsoft.AspNetCore.Mvc;
using AquaFarmApp.Data;
using AquaFarmApp.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AquaFarmApp.ViewModels;

namespace AquaFarmApp.Controllers
{
    public class FarmController : Controller
    {
        private readonly AquaFarmContext _context;
        public FarmController(AquaFarmContext context)
        {
            _context = context;
        }
        private List<string> GetStatus() => new() { "Avail", "Not Avail", "Health Secured" };
        private List<string> GetWaterType() => new() { "Freshwater", "Brackish water", "Saltwater", "Recirculated water", "Treated water" };

        public IActionResult Menu()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.StatusList = GetStatus();
            ViewBag.WaterList = GetWaterType();
            return View(new CreateFarmViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateFarmViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = GetStatus();
                ViewBag.WaterList = GetWaterType();
                return View(model);
            }

            var farm = new Farm
            {
                FarmName = model.FarmName,
                FarmLocation = model.FarmLocation,
                AreaTotal = model.AreaTotal,
                CreatedAt = DateTime.Now
            };

            _context.Farms.Add(farm);
            await _context.SaveChangesAsync();

            // Gán FarmId cho các Area
            foreach (var area in model.Areas)
            {
                area.FarmId = farm.FarmId;
                _context.Areas.Add(area);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var farms = await _context.Farms.Include(f => f.Areas).ToListAsync();
            return View(farms);
        }

        public async Task<IActionResult> Details(int id)
        {
            var farm = await _context.Farms.Include(f => f.Areas).FirstOrDefaultAsync(f => f.FarmId == id);
            if (farm == null) return NotFound();
            return View(farm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var farm = await _context.Farms.FindAsync(id);
            if (farm == null) return NotFound();

            return View(farm);
        }

        public async Task<IActionResult> Edit(int id, Farm updatedFarm)
        {
            if (id != updatedFarm.FarmId) return NotFound();

            if (!ModelState.IsValid) return View(updatedFarm);

            var existingFarm = await _context.Farms.FindAsync(id);
            if (existingFarm == null) return NotFound();

            existingFarm.FarmName = updatedFarm.FarmName;
            existingFarm.FarmLocation = updatedFarm.FarmLocation;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var farm = await _context.Farms.Include(f => f.Areas).FirstOrDefaultAsync(f => f.FarmId == id);

            if (farm != null)
            {
                _context.Areas.RemoveRange(farm.Areas);
                _context.Farms.Remove(farm);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Farm");
        }
    }
} 