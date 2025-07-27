using Microsoft.AspNetCore.Mvc;
using AquaFarmApp.Data;
using AquaFarmApp.Models;

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
            ViewBag.TypeOfWaterList = new List<string> { "Nước ngọt", "Nước lợ", "Nước mặn", "Nước tuần hoàn", "Nước đã qua xử lý" };

            return View();
        }

        [HttpPost]
        public IActionResult CreateAreas(List<Area> areas)
        {
            if (ModelState.IsValid)
            {
                foreach (var area in areas)
                {
                    if (string.IsNullOrEmpty(area.AreaStatus))
                        area.AreaStatus = "Avail";
                    _context.Areas.Add(area);
                }
                _context.SaveChanges();
                return RedirectToAction("Menu", "Farm");
            }
            return View(areas);
        }

        public IActionResult Find()
        {
            ViewBag.AreaStatusList = new List<string> { "Avail", "Not Avail", "Health Secured" };
            ViewBag.TypeOfWaterList = new List<string> { "Nước ngọt", "Nước lợ", "Nước mặn", "Nước tuần hoàn", "Nước đã qua xử lý" };
            return View();
        }

        [HttpPost]
        public IActionResult Find(string areaStatus, string typeOfWater)
        {
            var areas = _context.Areas
                .Where(a => (string.IsNullOrEmpty(areaStatus) || a.AreaStatus == areaStatus)
                         && (string.IsNullOrEmpty(typeOfWater) || a.TypeOfWater == typeOfWater))
                .ToList();
            return View("FindResult", areas);
        }
    }
}