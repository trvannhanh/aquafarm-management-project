using AquaFarmApp.Data;
using AquaFarmApp.Models;
using AquaFarmApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            return RedirectToAction("Index", "Home");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Farm updatedFarm)
        {
            if (id != updatedFarm.FarmId) return NotFound();
            if (!ModelState.IsValid) return View(updatedFarm);

            var existingFarm = await _context.Farms.FindAsync(id);
            if (existingFarm == null) return NotFound();

            try
            {
                existingFarm.FarmName = updatedFarm.FarmName;
                existingFarm.FarmLocation = updatedFarm.FarmLocation;
                existingFarm.AreaTotal = updatedFarm.AreaTotal; // Cập nhật AreaTotal nếu cần
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật trang trại thành công.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Lỗi khi cập nhật trang trại.";
                return View(updatedFarm);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int FarmId)
        {
            var farm = await _context.Farms
                .Include(f => f.Areas)
                .Include(f => f.Users)
                .FirstOrDefaultAsync(f => f.FarmId == FarmId);

            if (farm == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy trang trại.";
                return RedirectToAction("Index", "Home");
            }

            if (farm.Users.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa trang trại do có nhân viên liên kết.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                _context.Areas.RemoveRange(farm.Areas);
                _context.Farms.Remove(farm);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa trang trại thành công.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Lỗi khi xóa trang trại.";
            }

            return RedirectToAction("Index", "Home");
        }
    }
} 