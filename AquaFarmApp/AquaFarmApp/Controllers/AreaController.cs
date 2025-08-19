using AquaFarmApp.Data;
using AquaFarmApp.Models;
using AquaFarmApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost]

        private List<string> GetStatus() => new() { "Avail", "Not Avail", "Health Secured" };
        private List<string> GetWaterType() => new() { "Freshwater", "Brackish water", "Saltwater", "Recirculated water", "Treated water" };

        [HttpPost]
        public async Task<IActionResult> Find(string areaStatus, string typeOfWater)
        {
            var areas = await _context.Areas.Where(a => (string.IsNullOrEmpty(areaStatus) || a.AreaStatus == areaStatus) && (string.IsNullOrEmpty(typeOfWater) || a.TypeOfWater == typeOfWater)).ToListAsync();
            return View("FindResult", areas);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null) return NotFound();

            ViewBag.StatusList = GetStatus();
            ViewBag.TypeOfWaterList = GetWaterType();

            return View(area);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Area updatedArea)
        {
            if (id != updatedArea.AreaId) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewBag.StatusList = GetStatus();
                ViewBag.TypeOfWaterList = GetWaterType();
                return View(updatedArea);
            }

            var area = await _context.Areas.FindAsync(id);
            if (area == null) return NotFound();

            area.AreaName = updatedArea.AreaName;
            area.AreaStatus = updatedArea.AreaStatus;
            area.TypeOfWater = updatedArea.TypeOfWater;
            area.AreaSize = updatedArea.AreaSize;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Area", new { farmId = area.FarmId });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var area = await _context.Areas.FindAsync(id);

            var farm = await _context.Farms.FindAsync(area.FarmId);
            if (farm != null && farm.AreaTotal > 0)
            {
                farm.AreaTotal--;
            }
            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Area", new { farmId = area.FarmId });
        }

        public async Task<IActionResult> Index(int farmId)
        {
            ViewBag.AreaStatusList = GetStatus();
            ViewBag.TypeOfWaterList = GetWaterType();

            var areas = await _context.Areas.Where(a => farmId.Equals(a.FarmId)).Select(a => new AreaIndexViewModel
            {
                AreaId = a.AreaId,
                AreaName = a.AreaName,
                AreaStatus = a.AreaStatus,
                TypeOfWater = a.TypeOfWater,
                EnvWarn = a.EnvironmentLogs.OrderByDescending(e => e.RecordedAt).Select(e => e.IsWarning).FirstOrDefault()
            }).ToListAsync();
            return View(areas);
        }



    }
}