using Microsoft.AspNetCore.Mvc;
using AquaFarmApp.Data;
using AquaFarmApp.Models;
using Microsoft.EntityFrameworkCore;
using AquaFarmApp.ViewModels;
using Microsoft.VisualBasic;

namespace AquaFarmApp.Controllers
{
    public class FeedScheduleController : Controller
    {
        private readonly AquaFarmContext _context;

        public FeedScheduleController(AquaFarmContext context)
        {
            _context = context;
        }
        private List<string> GetFeedTypes() => new() { "Thức ăn viên công nghiệp", "Thức ăn tự chế", "Cám gạo", "Bột cá", "Trùn chỉ" };
        [HttpGet]
        public IActionResult Create(int id)
        {
            var model = new CreateFeedScheduleViewModel
            {
                AreaBatchId = id
            };
            ViewBag.FeedTypeList = GetFeedTypes();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create (CreateFeedScheduleViewModel model)
        {
            if(!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine("ModelState Errors: " + string.Join(", ", errors));
                ViewBag.FeedTypeList = GetFeedTypes();
                ViewBag.FeedTypeList = GetFeedTypes();
                return View(model);
            }
            var areaBatch = await _context.AreaBatches.FirstOrDefaultAsync(ab => ab.Id == model.AreaBatchId);
            var feedS = new FeedSchedule
            {
                FeedType = model.FeedType,
                FeedCost = model.FeedCost,
                FeedQuantity = model.QuantityKg,
                FeedTime= model.FeedTime,
                Note = model.Note,
                FeedStatus = "Not Done",
                AreabatchId = model.AreaBatchId
            };
            _context.FeedSchedules.Add(feedS);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Area");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var feedSchedules = await _context.FeedSchedules.Include(f => f.Areabatch).Include(f => f.User).ToListAsync();
            return View(feedSchedules);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var feedSchedule = await _context.FeedSchedules.FindAsync(id);
            if (feedSchedule == null) return NotFound();
            return View(feedSchedule);
        }
        public async Task<IActionResult> Edit(int id, FeedSchedule feedS)
        {
            if(id != feedS.FeedId) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(feedS);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var feedSchedule = await _context.FeedSchedules.FindAsync(id);
            if (feedSchedule == null) return NotFound();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsDone(int id)
        {
            var feed = await _context.FeedSchedules.FindAsync(id);
            if (feed == null)
            {
                return NotFound();
            }

            feed.FeedStatus = "Done";
            _context.Update(feed);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
