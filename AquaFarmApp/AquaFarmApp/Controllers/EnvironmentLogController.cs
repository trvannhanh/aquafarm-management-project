using Microsoft.AspNetCore.Mvc;
using AquaFarmApp.Data;
using AquaFarmApp.Models;
using Microsoft.EntityFrameworkCore;
using AquaFarmApp.ViewModels;
using Microsoft.VisualBasic;

namespace AquaFarmApp.Controllers
{
    public class EnvironmentLogController : Controller
    {
        private readonly AquaFarmContext _context;

        public EnvironmentLogController(AquaFarmContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Create(int id)
        {
            var model = new CreateEnvLogViewModel
            {
                AreaId = id
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEnvLogViewModel model)
        {
            if (ModelState.IsValid)
            {
                var env = new EnvironmentLog
                {
                    Temperature = model.Temperature,
                    PhLevel = model.PhLevel,
                    OxygenLevel = model.OxygenLevel,
                    Salinity = model.Salinity,
                    RecordedAt = DateTime.Now,
                    Note = model.Note,
                    AreaId = model.AreaId,
                };
                if(model.Temperature < 15 || model.Temperature > 35 ||
                   model.PhLevel < 6 || model.PhLevel > 8.5 ||
                   model.OxygenLevel < 5 || model.Salinity > 30 )
                {
                    env.IsWarning = true;
                }
                else
                {
                    env.IsWarning = false;
                }
                _context.EnvironmentLogs.Add(env);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Area");
            }
            return View(model);
        }
    }
}
