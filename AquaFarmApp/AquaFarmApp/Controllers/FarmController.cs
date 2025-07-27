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
        public IActionResult Create(Farm farm)
        {
            if (ModelState.IsValid)
            {
                _context.Farms.Add(farm);
                _context.SaveChanges();

                TempData["CreatedFarmId"] = farm.FarmId;
                TempData["AreaTotal"] = farm.AreaTotal;
                return RedirectToAction("CreateAreas", "Area");
            }
            return View(farm);
        }

        public IActionResult Index()
        {
            var farms = _context.Farms.Include(f => f.Areas).ToList();
            return View(farms);
        }

        public IActionResult Details(int id)
        {
            var farm = _context.Farms.Include(f => f.Areas).FirstOrDefault(f => f.FarmId == id);
            if (farm == null) return NotFound();
            return View(farm);
        }
    }
} 