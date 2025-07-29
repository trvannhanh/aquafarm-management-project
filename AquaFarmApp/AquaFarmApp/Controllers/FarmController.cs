using Microsoft.AspNetCore.Mvc;
using AquaFarmApp.Data;
using AquaFarmApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Controllers
{
    public class FarmController : Controller
    {
        private readonly AquaFarmContext _context;
        public FarmController(AquaFarmContext context)
        {
            _context = context;
        }

        public IActionResult Menu()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Farm farm)
        {
            if (ModelState.IsValid)
            {
                _context.Farms.Add(farm);
                await _context.SaveChangesAsync();
                farm.CreatedAt = DateTime.Now;
                TempData["CreatedFarmId"] = farm.FarmId;
                TempData["AreaTotal"] = farm.AreaTotal;
                return RedirectToAction("CreateAreas", "Area");
            }
            return View(farm);
        }

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
    }
} 