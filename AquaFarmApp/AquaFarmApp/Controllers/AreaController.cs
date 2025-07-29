using Microsoft.AspNetCore.Mvc;
using AquaFarmApp.Data;
using AquaFarmApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AquaFarmApp.Controllers
{
    public class AreaController : Controller
    {
        private readonly AquaFarmContext _context;

        public AreaController(AquaFarmContext context)
        {
            _context = context;
        }

        public IActionResult CreateAreas()
        {
            int areaTotal = (int)(TempData["AreaTotal"] ?? 0);
            int farmId = (int)(TempData["CreatedFarmId"] ?? 0);

            ViewBag.AreaTotal = areaTotal;
            ViewBag.FarmId = farmId;
            ViewBag.AreaStatusList = new List<string> { "Avail", "Not Avail", "Health Secured" };
            ViewBag.TypeOfWaterList = new List<string> { "Freshwater", "Brackish water", "Saltwater", "Recirculated water", "Treated water" };

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAreas(List<Area> areas)
        {
            if (ModelState.IsValid)
            {
                foreach (var area in areas)
                {
                    if (string.IsNullOrEmpty(area.AreaStatus))
                        area.AreaStatus = "Avail";
                    _context.Areas.Add(area);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Menu", "Farm");
            }
            return View(areas);
        }

        public IActionResult Find()
        {
            ViewBag.AreaStatusList = new List<string> { "Avail", "Not Avail", "Health Secured" };
            ViewBag.TypeOfWaterList = new List<string> { "Freshwater", "Brackish water", "Saltwater", "Recirculated water", "Treated water" };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Find(string areaStatus, string typeOfWater)
        {
            var areas = await _context.Areas.Where(a => (string.IsNullOrEmpty(areaStatus) || a.AreaStatus == areaStatus) && (string.IsNullOrEmpty(typeOfWater) || a.TypeOfWater == typeOfWater)).ToListAsync();
            return View("FindResult", areas);
        }
    }
}